/*
 * ReservationPageHandler
 * 
 * Description: A class to store methods and pages related to placing, updating, and canceling Reservations.
 * 
 * Changelog:
 * 4/23/2022: Initial commit - Alex
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
			Tuple.Create<Func<String, String>, String>(InputStartDate, "Enter the start date (determines the reservation types availble)");

		private readonly static Tuple<Func<String, String>, String> updatedStartDateRequest =
			Tuple.Create<Func<String, String>, String>(InputStartDate, "Enter the new start date (determines the reservation types availble)");

		private readonly static Tuple<Func<String, String>, String> newEndDateRequest =
			Tuple.Create<Func<String, String>, String>(InputEndDate, "Enter the end date");

		private readonly static Tuple<Func<String, String>, String> updatedEndDateRequest =
			Tuple.Create<Func<String, String>, String>(InputEndDate, "Enter the new end date");

		private readonly static Tuple<Func<String, String>, String> reservationTypeRequest =
			Tuple.Create<Func<String, String>, String>(InputType, "Enter the desired reservation type (1: Conventional, 2: Incentive, 3: 60-days, 4: Prepaid)");

		private readonly static Tuple<Func<String, String>, String> newGuestNameRequest =
			Tuple.Create<Func<String, String>, String>(InputName, "Enter guest name (you may press <enter> to skip if already provided)");

		private readonly static Tuple<Func<String, String>, String> updatedGuestNameRequest =
			Tuple.Create<Func<String, String>, String>(InputName, "Enter guest name (press <enter> to skip)");

		private readonly static Tuple<Func<String, String>, String> newCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter credit card number (you may press <enter> to skip if already provided or for 60-days reservations)");

		private readonly static Tuple<Func<String, String>, String> updatedCreditCardRequest =
			Tuple.Create<Func<String, String>, String>(InputCreditCard, "Enter credit card number (press <enter> to skip)");

		private readonly static Tuple<Func<String, String>, String> newEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter email address (you may press <enter> to skip unless you skipped Step 5 and the information has not been provided)");

		private readonly static Tuple<Func<String, String>, String> updatedEmailRequest =
			Tuple.Create<Func<String, String>, String>(InputEmail, "Enter email adddress (press <enter> to skip)");

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
				}, AddBufferToDB);

			changeRes = new ProcessPage("Change Reservation", "Change the dates and type of an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedStartDateRequest, updatedEndDateRequest, reservationTypeRequest, newCreditCardRequest, newEmailRequest
				}, ReplaceReferenceWithBuffer);

			changeGuestInfo = new ProcessPage("Change Guest Information", "Change the guest information for an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					guestNameSearchRequest, selectionSearchRequest, updatedGuestNameRequest, updatedCreditCardRequest, updatedEmailRequest
				}, CopyBufferToReference);

			cancelRes = new ProcessPage("Cancel Reservation", "Cancel an existing reservation",
				new List<Tuple<Func<String, String>, String>> {
					Tuple.Create<Func<String, String>, String>(InputStartDate, "Enter the start date (determines the reservation types availble)")
				}, CopyBufferToReference);

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
				return $"No reservations under the name \"{input}\"";//REMOVE AFTER MERGE
			}

			// Display search results
			Console.WriteLine($"The reservations for {input} are as follows:");

			for (int i = 0; i < searchResults.Count; i++)
			{
				Console.WriteLine($"{i + 1}: {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()})");
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
			referenceRes = bufferRes = searchResults[selection - 1];
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
			bufferRes.setStartDate(startDate);
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
			if (endDate < bufferRes.getStartDate())
            {
				return $"{endDate.ToShortDateString()} is before start date ({bufferRes.getStartDate().ToShortDateString()})";
			}
			if (cal.retrieveDate(endDate).IsFull())
            {
				return $"Hotel is full on {endDate.ToShortDateString()}";
			}

			// Check availibility
			Console.Write("Confirming availibility... ");

			// Ensure space is availible on all intermediate days
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
			int daysAdvance = (int) (bufferRes.getStartDate() - DateTime.Today).TotalDays;
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
					for (DateTime d = bufferRes.getStartDate(); d <= bufferRes.getEndDate(); d = d.AddDays(1))
                    {
						totalOccupancy += cal.retrieveDate(d).getOccupancy();
					}
					totalOccupancyPercent = (double)totalOccupancy / (1.0 + (bufferRes.getEndDate() - bufferRes.getStartDate()).TotalDays);
					if (totalOccupancyPercent > 0.6)
                    {
						return $"Incentive reservations are only allowed when the avergage occupancy is under 60% (currently {100 * totalOccupancy}%)";
					}

					// Accept otherwise
					discount = 0.20;
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
					break;

				// Invalid selection
				default: return $"{input} is not between 1 and 4";
			}

			// Calculate the base 
			Console.Write("Calculating price... ");
			double cost = 0;
			for (DateTime d = bufferRes.getStartDate(); d <= bufferRes.getEndDate(); d = d.AddDays(1))
            {
				cost += cal.retrieveDate(d).getBasePrice();
			}

			// Apply discount
			cost *= (1.0 - discount);
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
			bufferRes.setCustomerName(input);
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
			if (sets.Length == 0 && (bufferRes.getReservationType() == ReservationType.SixtyDay || bufferRes.getCustomerCreditCard() != ""))
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
		/// Save changes to a new reservation by adding the buffer to the database.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String AddBufferToDB()
		{
			rdb.addReservation(bufferRes);
			bufferRes = null;
			return "";
		}

		/// <summary>
		/// Save minor changes to an existing reservation by copying changes from the buffer to the reservation in the database.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String CopyBufferToReference()
		{
			bufferRes = referenceRes = null;
			return "";
		}

		/// <summary>
		/// Save major changes to an existing reservation by replacing the reference copy with the buffer copy.
		/// </summary>
		/// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
		static String ReplaceReferenceWithBuffer()
		{
			bufferRes = referenceRes = null;
			return "";
		}
	}
}

