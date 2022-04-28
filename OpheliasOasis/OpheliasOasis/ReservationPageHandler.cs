/*
 * ReservationPageHandler
 * 
 * Description: A class to store methods and pages related to placing, updating, and canceling Reservations.
 * 
 * Changelog:
 * 4/23/2022: Initial commit - Alex
 * 4/24/2022: Added interface to reservationDB & changed code to treat end date as check-out date - Alex
 * 4/24/2022: Added code to handle refunds - Alex
 * 4/25/2022: Added code to update occupancy - Nathan
 * 4/26/2022: Kept code from charging guest for changes made to a conventional or incentive reservation & updated to work with new reservation methods - Alex
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
	public static class ReservationPageHandler
	{
		// Variables for passing information between steps
		private static Calendar cal;
		private static ReservationDB rdb;
		private static Reservation? bufferRes, referenceRes;
		private static List<Reservation> searchResults;

		// Reservation submenu
		public static MenuPage resMenu;

		// Menu options
		public static ProcessPage placeRes;
		public static ProcessPage changeRes;
		public static ProcessPage changeGuestInfo;
		public static ProcessPage cancelRes;
		public static ProcessPage showDate;

		// Steps - tuples pairing an input-parsing function with the associated prompt - used in ProcessPages
		private readonly static Tuple<Func<String, String>, String> guestNameSearchRequest =
			Tuple.Create<Func<String, String>, String>(InputSearchName, "Enter the name the guest used to place the reservation");

		private readonly static Tuple<Func<String, String>, String> selectionSearchRequest =
			Tuple.Create<Func<String, String>, String>(InputResSelection, "Select one of the options above (enter the index of the left)");

		private readonly static Tuple<Func<String, String>, String> newStartDateRequest =
			Tuple.Create<Func<String, String>, String>(InputStartDate, "Enter the check-in date (determines the reservation types available)");

		private readonly static Tuple<Func<String, String>, String> updatedStartDateRequest =
			Tuple.Create<Func<String, String>, String>(InputStartDate, "Enter the new check-in date (determines the reservation types available)");

		private readonly static Tuple<Func<String, String>, String> newEndDateRequest =
			Tuple.Create<Func<String, String>, String>(InputEndDate, "Enter the checkout date");

		private readonly static Tuple<Func<String, String>, String> updatedEndDateRequest =
			Tuple.Create<Func<String, String>, String>(InputEndDate, "Enter the new checkout date");

		private readonly static Tuple<Func<String, String>, String> reservationTypeRequest =
			Tuple.Create<Func<String, String>, String>(InputType, "Enter the desired reservation type (1: Conventional, 2: Incentive, 3: 60-days, 4: Prepaid)");

		private readonly static Tuple<Func<String, String>, String> newGuestNameRequest =
			Tuple.Create<Func<String, String>, String>(InputName, "Enter guest name (press <enter> to leave unchanged)");

		private readonly static Tuple<Func<String, String>, String> updatedGuestNameRequest =
			Tuple.Create<Func<String, String>, String>(InputName, "Enter new guest name (press <enter> to leave unchanged)");

		private readonly static Tuple<Func<String, String>, String> newCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter credit card number (you may press <enter> to skip if already provided / for 60-days reservations)");

		private readonly static Tuple<Func<String, String>, String> updatedCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter new credit card number (press <enter> to leave unchanged)");

		private readonly static Tuple<Func<String, String>, String> newEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter email address (you may press <enter> to skip if credit card has been provided)");

		private readonly static Tuple<Func<String, String>, String> updatedEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter new email adddress (press <enter> to leave unchanged)");

		private readonly static Tuple<Func<String, String>, String> cancelrequest =
			Tuple.Create<Func<String, String>, String>(InputCancellationConfirmation, "Cancel reservation? (Y/n)");

		private readonly static Tuple<Func<String, String>, String> showDateReservations =
			Tuple.Create<Func<String, String>, String>(ShowByDate, "Enter a start date to show");

		/// <summary>
		/// Load the pages and store references to the variables they need.
		/// </summary>
		/// <param name="db">A reference to the main database.</param>
		/// <param name="cl">A reference to the main calendar</param>
		public static void Init(ReservationDB db, Calendar cl)
		{
			// Initialize references
			rdb = db;
			cal = cl;

			// Initialize menu options
			placeRes = new ProcessPage("Place Reservation", "Place a new reservation",
				new List<Tuple<Func<String, String>, String>> {
					newStartDateRequest, newEndDateRequest, reservationTypeRequest, newGuestNameRequest, newCreditCardRequest, newEmailRequest
				}, PlaceReservation, ClearBuffer);

			changeRes = new ProcessPage("Change Reservation", "Change the dates and type of an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedStartDateRequest, updatedEndDateRequest, reservationTypeRequest, newCreditCardRequest, newEmailRequest
				}, ChangeReservationDates, ClearBuffer);

			changeGuestInfo = new ProcessPage("Change Guest Information", "Change the guest information for an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedGuestNameRequest, updatedCreditCardRequest, updatedEmailRequest
				}, ChangeReservation, ClearBuffer);

			cancelRes = new ProcessPage("Cancel Reservation", "Cancel an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, cancelrequest
				}, CancelReservation, ClearBuffer);

			showDate = new ProcessPage("Show Reservations by Date", "Shows all reservations beginning on a given date",
				new List<Tuple<Func<String, String>, String>> { showDateReservations }, null, null);

			// Initialize menu
			resMenu = new MenuPage("Reservations", "Reservations submenu (place, update, or cancel a reservation)", new List<Page> { placeRes, changeRes, changeGuestInfo, cancelRes, showDate });
		}

		/// <summary>
		/// Search for reservations under the name specified in the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputSearchName(String input)
		{
			// Check for first and last name
			if (!input.Contains(" "))
			{
				return "First and last name required";
			}

			// Aquire search results
			searchResults = rdb.getReservation(input);

			// Check for empty search results
			if (searchResults == null || searchResults.Count < 1)
			{
				return $"No reservations under the name \"{input}\"";
			}

			// Display search results
			Console.WriteLine($"The reservations for {input} are as follows:");

			for (int i = 0; i < searchResults.Count; i++)
			{
				Console.WriteLine($"\t{i + 1}: {searchResults[i]}");
			}

			// Move on to next step
			return "";
		}

		/// <summary>
		/// Select the reservation based on the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputResSelection(String input)
		{
			// Read and validate the selection
			int selection;

			if (!int.TryParse(input, out selection) || selection > searchResults.Count || selection < 1)
			{
				return $"\"{input}\" is not between 1 and {searchResults.Count}";
			}

			// Read in selection
			referenceRes = searchResults[selection - 1];

			// Ensure the date is active
			if (referenceRes.getReservationStatus() == ReservationStatus.Cancelled)
            {
				return "You cannot modifiy a cancelled reservation";
            }

			// Store and coninue
			bufferRes = referenceRes.Clone();
			return "";
		}

		/// <summary>
		/// Start a new reservation on the date specified in the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputStartDate(String input)
		{
			// Don't allow changes if the existing start date has already passed
			if (referenceRes != null && bufferRes.getStartDate() < DateTime.Today)
            {
				return "The reservation has already started. Start date cannot be changed";
            }

			// Validate start date
			DateTime startDate;
			if (!DateTime.TryParse(input, out startDate))
			{
				return $"\"{input}\" is not a valid date";
			}
			if (startDate < DateTime.Today)
			{
				return $"{startDate.ToShortDateString()} is before today ({DateTime.Today.ToShortDateString()})";
			}
			if (cal.retrieveDate(startDate).IsFull())
			{
				return $"Hotel is full on {startDate.ToShortDateString()}";
			}

			// Set the buffer if this is a new reservation (buffer hasn't been set to a clone of the original)
			if (bufferRes == null)
			{
				bufferRes = new Reservation();
				bufferRes.setReservationStatus(ReservationStatus.Placed);
			}

			// Store and continue
			bufferRes = bufferRes.WithStartDate(startDate);
			return "";
		}

		/// <summary>
		/// Set the start date of the new reservation to the date specified in the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputEndDate(String input)
		{
			// Don't allow changes if the existing end date has already passed
			if (referenceRes != null && bufferRes.getEndDate() < DateTime.Today)
			{
				return "The reservation has already ended. End date cannot be changed";
			}

			// Validate end date
			DateTime endDate;
			if (!DateTime.TryParse(input, out endDate))
			{
				return $"\"{input}\" is not a valid date";
			}
			if (endDate <= bufferRes.getStartDate())
			{
				return $"{endDate.ToShortDateString()} is on or before before start date ({bufferRes.getStartDate().ToShortDateString()})";
			}

			// Check availibility
			Console.Write("Confirming availibility... ");

			// Ensure space is available on all intermediate days
			for (DateTime d = bufferRes.getStartDate(); d < endDate; d = d.AddDays(1))
			{
				if (cal.retrieveDate(d).IsFull()) return $"Hotel is full on {d.ToShortDateString()}";
			}

			// Confirm availibility
			Console.WriteLine("Availibility confirmed.");

			// Store and continue
			bufferRes.setEndDate(endDate);
			return "";
		}

		/// <summary>
		/// Set the reservation type from the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputType(String input)
		{
			// Determine the number of days ahead the reservation is being requested
			ReservationType type = ReservationType.Conventional;
			int daysAdvance = (int)(bufferRes.getStartDate() - DateTime.Today).TotalDays;
			double discount = 0;

			// Validate selection
			switch (input)
			{
				// Attempt to leave unchanged
				case "":
					Console.WriteLine($"Type left as {bufferRes.getReservationType()} Reservation.");
					break;

				// Conventional
				case "1":
					break;

				// Incentive
				case "2":

					// Reject days too far in advance
					if (daysAdvance >= 30) return $"Incentive reservations must be made less than 30 days in advance, not {daysAdvance} days";

					// Reject days with too large an occupancy
					int totalOccupancy = 0;
					double totalOccupancyPercent;
					for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
					{
						totalOccupancy += cal.retrieveDate(d).getOccupancy();
					}
					totalOccupancyPercent = (double)totalOccupancy / (bufferRes.getEndDate() - bufferRes.getStartDate()).TotalDays;
					if (totalOccupancyPercent > 0.6)
					{
						return $"Incentive reservations are only allowed when the avergage occupancy is under 60.00% (currently {totalOccupancy:P})";
					}

					// Accept otherwise
					discount = 0.20;
					type = ReservationType.Incentive;
					break;

				// Sixty Day
				case "3":

					// Reject if not enough advance notice is provided
					if (daysAdvance < 60)
					{
						return $"60 days advance reservations can only be made 60 days in advance, not {daysAdvance} days";
					}

					// Accept otherwise
					discount = 0.15;
					type = ReservationType.SixtyDay;
					break;

				// Prepaid
				case "4":

					// Reject if not enough advance notice is provided
					if (daysAdvance < 90)
					{
						return $"Prepaid reservations can only be made 90 days in advance, not {daysAdvance} days";
					}

					// Accept otherwise
					discount = 0.25;
					type = ReservationType.Prepaid;
					break;

				// Invalid selection
				default:
					return $"\"{input}\" is not between 1 and 4";
			}

			// Change discount to penalty if changing a Prepaid or SixtyDays reservation
			if (referenceRes != null && (referenceRes.getReservationType() == ReservationType.Prepaid || referenceRes.getReservationType() == ReservationType.SixtyDay))
            {
				discount = -0.10;
            }

			// Calculate the price
			Console.Write("Calculating price... ");
			List<double> prices = new List<double>();

			// Store price
			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				prices.Add((1.0 - discount) * cal.retrieveDate(d).getBasePrice());
			}

			bufferRes.SetPrices(prices);

			// Display the price
			if (Math.Abs(discount) > 0.001)
			{
				Console.Write($"Applied {Math.Abs(discount):P} {(discount < 0 ? "penalty" : "discount")}. ");
			}
			Console.WriteLine($"Calculated final price: {bufferRes.GetTotalPrice():C2}.");

			// Store type and continue
			bufferRes.setReservationType(type);
			return "";
		}

		/// <summary>
		/// Set the name for the reservation to the name specified in the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputName(String input)
		{
			// Parse input
			if (String.IsNullOrEmpty(input) && !String.IsNullOrEmpty(bufferRes.getCustomerName()))
			{
				// Skip if requested and allowed
				Console.WriteLine($"Name left as \"{bufferRes.getCustomerName()}\".");
				return "";
			}
			else if (!input.Contains(" "))
			{
				// Check for first and last name
				return "First and last name required";
			}

			// Store and continue
			bufferRes = bufferRes.WithCustomerName(input);
			return "";
		}

		/// <summary>
		/// Set the customer credit card information for reservation from the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputCreditCard(String input)
		{
			// Check for 4 sets of 4 digits
			string[] sets = input.Split(" ");

			// Skip if requested and allowed
			if (String.IsNullOrEmpty(input) && (bufferRes.getReservationType() == ReservationType.SixtyDay || !String.IsNullOrEmpty(bufferRes.getCustomerCreditCard())))
			{
				if (!String.IsNullOrEmpty(bufferRes.getCustomerCreditCard()))
				{
					Console.WriteLine($"Credit card left as XXXX XXXX XXXX {bufferRes.getCustomerCreditCard().Split(" ")[3]}.");
				}
				else
				{
					Console.WriteLine("Credit card skipped.");
				}
				return "";
			}

			// Don't allow changes after the reservation has been paid
			if (bufferRes.IsPaid())
            {
				return "The reservation has been paid for. You cannot change the payment information";
            }

			// Catch imporper spacing
			if (sets.Length != 4)
			{
				return "Format must be XXXX XXXX XXXX XXXX";
			}

			// Ensure the input is a sequence of 4 sets of 4 numbers
			for (int i = 0; i < 4; i++)
			{
				if (sets[i].Length != 4)
				{
					return "Format must be XXXX XXXX XXXX XXXX";
				}

				for (int j = 0; j < 4; j++)
				{
					if (!Char.IsDigit(sets[i][j]))
					{
						return $"\"{sets[i][j]}\" is not a number";
					}
				}
			}

			// Store and continue
			bufferRes.setCustomerCreditCard(input);
			bufferRes.setReservationStatus(ReservationStatus.Confirmed);
			return "";
		}

		/// <summary>
		/// Set the customer email for reservation from the user's response to the prompt.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputEmail(String input)
		{
			// Parse input
			if (String.IsNullOrEmpty(input) && (bufferRes.getReservationType() != ReservationType.SixtyDay || !String.IsNullOrEmpty(bufferRes.getCustomerCreditCard())))
			{
				// Skip if requested and allowed
				if (!String.IsNullOrEmpty(bufferRes.getCustomerEmail()))
				{
					Console.WriteLine($"Email address left as \"{bufferRes.getCustomerEmail()}\".");
				}
				else
                {
					Console.WriteLine("Email skipped.");
                }
				return "";
			}
			else if (!input.Contains("@") || !input.Contains("."))
			{
				// Check for values that clearly aren't emails
				return $"\"{input}\" is not a vald email address";
			}

			// Store and continue
			bufferRes.setCustomerEmail(input);
			return "";
		}

		/// <summary>
		/// Cancel a reservation.
		/// </summary>
		/// <param name="input">A string containing the user's reponse to the prompt.</param>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String InputCancellationConfirmation(String input)
		{
			if (input == "N")
			{
				return "Press \"Q\" to quit or \"B\" to select a different reservation";
			}
			else
			{
				return "";
			}
		}

		static String ClearBuffer()
        {
			bufferRes = referenceRes = null;
			return "";
        }

		/// <summary>
		/// Save changes to a new reservation by adding the buffer to the database.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String PlaceReservation()
		{
			// Make payments for prepaid and 60-days with credit card information provided
			if (bufferRes.getReservationType() == ReservationType.Prepaid || (bufferRes.getReservationType() == ReservationType.SixtyDay && !String.IsNullOrEmpty(bufferRes.getCustomerCreditCard())))
			{
				CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", bufferRes.GetTotalPrice());
				bufferRes.SetPaid(true);
			}

			// Update occupancy
			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).increaseOccupancy();
			}

			// Save & return
			rdb.addReservation(bufferRes);
			return ClearBuffer();
		}

		/// <summary>
		/// Save changes to an existing reservation by swapping the original with the copy containing the changes. Apply charges to 60-days.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String ChangeReservation()
		{
			if (bufferRes.getReservationType() == ReservationType.SixtyDay && !bufferRes.IsPaid() && !String.IsNullOrEmpty(bufferRes.getCustomerCreditCard()))
            {
				CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", bufferRes.GetTotalPrice());
				bufferRes.SetPaid(true);
			}

			// Save & return
			rdb.replaceReservation(referenceRes, bufferRes);
			return ClearBuffer();
		}

		/// <summary>
		/// Change the dates for an existing reservation and apply applicable fees.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String ChangeReservationDates()
		{
			// Compute price difference
			double priceDifference = bufferRes.GetTotalPrice() - referenceRes.GetTotalPrice();

			// No refunds for Perpaid or SixtyDays
			if (bufferRes.getReservationType() == ReservationType.Prepaid || bufferRes.getReservationType() == ReservationType.SixtyDay)
            {
				priceDifference = priceDifference < 0 ? 0 : priceDifference;
            }

			// Make corrections if the price has been paid
			if (referenceRes.IsPaid() && priceDifference > 0)
            {
				CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", priceDifference);
			}

			// Update occupancy
			for (DateTime d = referenceRes.getStartDate(); d < referenceRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).decreaseOccupancy();
			}
			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).increaseOccupancy();
			}

			// Save & return
			return ChangeReservation();
		}

		/// <summary>
		/// Cancel a reservation and apply applicable refunds.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String CancelReservation()
		{
			// Apply applicable refunds
			if (bufferRes.getReservationType() == ReservationType.Conventional || bufferRes.getReservationType() == ReservationType.Incentive)
			{
				double refund = (bufferRes.getStartDate() - DateTime.Today).TotalDays > 3 ? bufferRes.GetTotalPrice() : bufferRes.GetTotalPrice() - bufferRes.GetFirstDayPrice();
				CreditCardStub.WriteTransaction("Ophelia's Oasis", "1234 1234 1234 1234", bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), refund);
			}
			
			// Update occupancy
			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).decreaseOccupancy();
			}

			// Save & return
			bufferRes.cancelReservation();
			return ChangeReservation();
		}


		static String ShowByDate(String input)
		{
			// Validate date
			DateTime displayDate;
			if (!DateTime.TryParse(input, out displayDate))
			{
				return $"\"{input}\" is not a valid date";
			}

			List<Reservation> dateReservations = rdb.getReservation(displayDate);

			Console.WriteLine("Reservations on " + displayDate.ToShortDateString() + ": ");
            foreach (Reservation r in dateReservations) 
			{
				Console.WriteLine( "\t" + r.getCustomerName() + ": " + r);
			}
			return "";
		}


	}
}

