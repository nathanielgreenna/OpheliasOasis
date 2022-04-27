/*
 * CheckInPageHandler
 * 
 * Description: A class to store methods and pages related to Check-Ins.
 * 
 * Changelog:
 * 4/23/2022: Initial code - Alec
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{

    /// <summary>
    /// A class to provide funcionality for the check-in page.
    /// </summary>
    public static class CheckInPageHandler
    {
        private static ReservationDB ResDB;
        private static Hotel Htl;
        private static ProcessPage checkIn;
        private static List<Reservation> searchResults;
        private static Reservation? referenceRes;

        private readonly static Tuple<Func<String, String>, String> guestNameSearchRequest =
            Tuple.Create<Func<String, String>, String>(InputSearchName, "Enter the name the guest used to place the reservation");
        private readonly static Tuple<Func<String, String>, String> selectionSearchRequest =
            Tuple.Create<Func<String, String>, String>(InputResSelection, "Select one of the options above (enter the index of the left)");



        public static void Init(ReservationDB db, Hotel htl)
        {
            // Initialize references
            ResDB = db;
            Htl = htl;

            // Initialize page
            checkIn = new ProcessPage("Check In", "Check In", new List<Tuple<Func<String, String>, String>>{ guestNameSearchRequest, selectionSearchRequest }, CheckInConfirm);
        }

        /// <summary>
        /// A method that returns the check-in page.
        /// </summary>
        /// <returns>The check-in page.</returns>
        public static ProcessPage getPage()
        {
            return checkIn;
        }

        /// <summary>
        /// A method that finds the reservation for a customer, and if found, checks them in.
        /// </summary>
        /// <param name="input">A string containing the input.</param>
        /// <returns>A string containing the reason why the input is not valid, if applicable. Otherwise a success message.</returns>

        static String CheckInConfirm()
        {
            if(referenceRes.getRoomNumber() <= 0)
            {
                referenceRes.setRoomNumber(Htl.assignRoom());
            }
            referenceRes.checkIn();
            Console.Write(referenceRes.getCustomerName() + " has been checked into room " + referenceRes.getRoomNumber());
            System.Threading.Thread.Sleep(3000);
            return "";
        }


        static String InputSearchName(String input)
        {
            // Check for first and last name
            if (!input.Contains(" "))
            {
                return "First and last name required";
            }

            // Aquire search results
            searchResults = ResDB.getReservation(input);

            for (int i = 0; i < searchResults.Count; i++)
            {
                if(searchResults[i].getStartDate() != DateTime.Today)
                {
                    searchResults.RemoveAt(i);
                }
            }



            if (searchResults == null || searchResults.Count < 1)
            {
                return $"No reservations under the name \"{input}\" today";
            }


            // Display search results
            Console.WriteLine($"Reservations for {input} available for check-in today:");
            int g = 0;

            for (int i = 0; i < searchResults.Count; i++)
            {
                if ((searchResults[i].getReservationStatus() == ReservationStatus.Paid || searchResults[i].getReservationStatus() == ReservationStatus.Placed)) 
                {
                    Console.WriteLine($"\t{g + 1}: {searchResults[i].getReservationType()} Reservation from {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()}, Credit Card #: {searchResults[i].getCustomerCreditCard()})");
                    g++;
                }
            }

            Console.WriteLine($"\nOther Reservations for {input} unavailable or already checked-in today:");



            for (int i = searchResults.Count-1; i >= 0; i--)
            {
                if (!(searchResults[i].getReservationStatus() == ReservationStatus.Paid || searchResults[i].getReservationStatus() == ReservationStatus.Placed))
                {
                    Console.WriteLine($"\t{searchResults[i].getReservationType()} Reservation from {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()}, Credit Card #: {searchResults[i].getCustomerCreditCard()})");
                    searchResults.RemoveAt(i);
                }
            }



            // Move on to next step
            return "";
        }

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

            if (referenceRes.getRoomNumber() <= 0)
            {
                Console.Write(referenceRes.getCustomerName() + " has not been assigned a room yet. Customer will be checked in, and a room will be assigned and displayed upon saving");
            }
            else
            {
                Console.Write(referenceRes.getCustomerName() + "is assigned to room " + referenceRes.getRoomNumber() + ". Save to check in");
            }
            return "";
        }







    }
}


