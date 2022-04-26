/*
 * ReservationPageHandler
 * 
 * Description: A class to store methods and pages related to placing, updating, and canceling Reservations.
 * 
 * Changelog:
 * 4/23/2022: Initial commit - Alex
 * 4/24/2022: Added interface to reservationDB & changed code to treat end date as check-out date - Alex
 * 4/24/2022: Added code to handle refunds - Alex
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
			Tuple.Create<Func<String, String>, String>(InputName, "Enter guest name (you may press <enter> to skip if already provided)");

		private readonly static Tuple<Func<String, String>, String> updatedGuestNameRequest =
			Tuple.Create<Func<String, String>, String>(InputName, "Enter new guest name (or press <enter> to skip)");

		private readonly static Tuple<Func<String, String>, String> newCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter credit card number (you may press <enter> to skip if already provided or for 60-days reservations)");

		private readonly static Tuple<Func<String, String>, String> updatedCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter new credit card number (or press <enter> to skip)");

		private readonly static Tuple<Func<String, String>, String> newEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter email address (you may press <enter> to skip unless you skipped Step 5 and the information has not been provided)");

		private readonly static Tuple<Func<String, String>, String> updatedEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter email new adddress (or press <enter> to skip)");

		private readonly static Tuple<Func<String, String>, String> cancelrequest =
			Tuple.Create<Func<String, String>, String>(InputCancellationConfirmation, "Cancel reservation? (Y/n)");

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
				}, PlaceReservation);

			changeRes = new ProcessPage("Change Reservation", "Change the dates and type of an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedStartDateRequest, updatedEndDateRequest, reservationTypeRequest, newCreditCardRequest, newEmailRequest
				}, ChangeReservationDates);

			changeGuestInfo = new ProcessPage("Change Guest Information", "Change the guest information for an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedGuestNameRequest, updatedCreditCardRequest, updatedEmailRequest
				}, ChangeReservation);

			cancelRes = new ProcessPage("Cancel Reservation", "Cancel an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, cancelrequest
				}, CancelReservation);

			// Initialize menu
			resMenu = new MenuPage("Reservations", "Reservations submenu (place, update, or cancel a reservation)", new List<Page> { placeRes, changeRes, changeGuestInfo, cancelRes });
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
				Console.WriteLine($"\t{i + 1}: {searchResults[i].getReservationType()} Reservation from {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()}, Credit Card #: {searchResults[i].getCustomerCreditCard()})");
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

			// Store and continue
			referenceRes = searchResults[selection - 1];
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

			// Store and continue
			if (bufferRes == null) bufferRes = new Reservation();
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
				default: return $"\"{input}\" is not between 1 and 4";
			}

			// Calculate the base 
			Console.Write("Calculating price... ");
			double cost = 0;
			double rate;
			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				// Store base rate for each date
				rate = cal.retrieveDate(d).getBasePrice();
				bufferRes.setDateRate(d, rate);
				// Apply discount and store discounted rate
				rate *= (1.0 - discount);
				cost += rate;
				bufferRes.setDateCost(d, rate);
			}

			bufferRes.setTotalPrice(cost);

			// Display the price
			if (discount > 0)
			{
				Console.Write($"Applied {discount:P} discount. ");
			}
			Console.WriteLine($"Calculated final price: {cost:C2}.");

			// Store and continue
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
				return "";
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
				bufferRes.cancelReservation();
				return "";
			}
		}

		/// <summary>
		/// Save changes to a new reservation by adding the buffer to the database.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String PlaceReservation()
		{
			if (String.IsNullOrEmpty(bufferRes.getCustomerCreditCard()) || bufferRes.getReservationType() == ReservationType.Conventional || bufferRes.getReservationType() == ReservationType.Incentive)
			{
				// Skip payment
				bufferRes.setReservationStatus(ReservationStatus.Placed);
			}
			else 
			{
				// Make payments for prepaid and 60-days with credit card information provided
				CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", bufferRes.getTotalPrice());
				bufferRes.setReservationStatus(ReservationStatus.Paid);
			}

			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).increaseOccupancy();
			}

			rdb.addReservation(bufferRes);
			bufferRes = null;
			return "";
		}

		/// <summary>
		/// Save changes to an existing reservation by swapping the original with the copy containing the changes.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String ChangeReservation()
		{
			rdb.replaceReservation(referenceRes, bufferRes);
			bufferRes = referenceRes = null;
			return "";
		}

		static String ChangeReservationDates()
		{
			// Conventional and incentive can be changed without penalty, unlike prepaid and 60-days advance reservations
			if (referenceRes.getReservationType() == ReservationType.Conventional || referenceRes.getReservationType() == ReservationType.Incentive)
            {
				double priceDifference = Math.Round(bufferRes.getTotalPrice() - referenceRes.getTotalPrice(), 2);

				if (priceDifference > 0)
                {
					CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", priceDifference);
                }
				else if (priceDifference < 0)
                {
					CreditCardStub.WriteTransaction("Ophelia's Oasis", "1234 1234 1234 1234", bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), -priceDifference);
				}

			} else
            {
				double discount = referenceRes.getReservationType() == ReservationType.Prepaid ? 0.25 : 0.15;
				double penalty = 1.1;
				double priceDifference = penalty / discount * bufferRes.getTotalPrice() - referenceRes.getTotalPrice();

				if (priceDifference > 0)
                {
					CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", priceDifference);
				}
            }

			for (DateTime d = referenceRes.getStartDate(); d < referenceRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).decreaseOccupancy();
			}

			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).increaseOccupancy();
			}


			// Save changes
			return ChangeReservation();
		}

		static String CancelReservation()
		{
			// Apply applicable refunds
			if (bufferRes.getReservationType() == ReservationType.Conventional || bufferRes.getReservationType() == ReservationType.Incentive)
			{
				double refund = (bufferRes.getStartDate() - DateTime.Today).TotalDays > 3 ? bufferRes.getTotalPrice() : bufferRes.getTotalPrice() - bufferRes.getFirstDayPrice();
				CreditCardStub.WriteTransaction(bufferRes.getCustomerName(), bufferRes.getCustomerCreditCard(), "Ophelia's Oasis", "1234 1234 1234 1234", refund);
			}

			for (DateTime d = bufferRes.getStartDate(); d < bufferRes.getEndDate(); d = d.AddDays(1))
			{
				cal.retrieveDate(d).decreaseOccupancy();
			}

			// Save changes
			return ChangeReservation();
		}
	}
}

