/*
 * CheckInPageHandler
 * 
 * Description: A class to store methods and pages related to Check-Outs.
 * 
 * Changelog:
 * 4/23/2022: Initial code - Alec
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    public static class CheckOutPageHandler
    {
        private static ReservationDB ResDB;
        private static Hotel Htl;
        private static ProcessPage checkOut;
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
            checkOut = new ProcessPage("Check Out", "Check Out", new List<Tuple<Func<String, String>, String>> { guestNameSearchRequest, selectionSearchRequest }, CheckOutConfirm, null);
        }

        /// <summary>
        /// A method that returns the check-out page.
        /// </summary>
        /// <returns>The check-out page.</returns>
        public static ProcessPage getPage()
        {
            return checkOut;
        }

        static String InputSearchName(String input)
        {
            // Check for first and last name
            if (!input.Contains(" "))
            {
                return "First and last name required";
            }

            int checkedincount = 0;
            // Aquire search results
            searchResults = ResDB.getReservation(input);

            for (int i = 0; i < searchResults.Count; i++)
            {
                if (searchResults[i].getReservationStatus() == ReservationStatus.CheckedIn)
                {
                    checkedincount++;
                }
            }



            if (checkedincount == 0)
            {
                return $"No reservations under the name \"{input}\" available for checkout";
            }


            // Display search results
            Console.WriteLine($"Reservations for {input} available for check-out today:");
            int g = 0;

            for (int i = 0; i < searchResults.Count; i++)
            {
                if (searchResults[i].getReservationStatus() == ReservationStatus.CheckedIn)
                {
                    Console.WriteLine($"\t{g + 1}: {searchResults[i].getReservationType()} Reservation from {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()}, Credit Card #: {searchResults[i].getCustomerCreditCard()})");
                    g++;
                }
            }

            Console.WriteLine($"\nOther Reservations for {input} unavailable or already checked out:");

            for (int i = searchResults.Count - 1; i >= 0; i--)
            {
                if (!(searchResults[i].getReservationStatus() == ReservationStatus.CheckedIn))
                {
                    if (searchResults[i].getEndDate() >= DateTime.Today)
                    { 
                        Console.WriteLine($"\t{searchResults[i].getReservationType()} Reservation from {searchResults[i].getStartDate().ToShortDateString()} to {searchResults[i].getEndDate().ToShortDateString()} ({searchResults[i].getReservationStatus()}, Credit Card #: {searchResults[i].getCustomerCreditCard()})");
                    }
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

            //continue
            referenceRes = searchResults[selection - 1];
            Console.WriteLine(referenceRes.getCustomerName() + " was assigned to room " + referenceRes.getRoomNumber() + ". Save to check out the guest.");
            
            return "";
        }


        /// <summary>
        /// A method that finds the reservation for a customer, and if found, checks them out.
        /// </summary>
        /// <param name="input">A string containing the input.</param>
        /// <returns>A string containing the reason why the input is not valid, if applicable. Otherwise a success message.</returns>
        public static String CheckOutConfirm()
        {
            if (referenceRes.getReservationType() == ReservationType.Conventional || referenceRes.getReservationType() == ReservationType.Incentive)
            {
                CreditCardStub.WriteTransaction(referenceRes.getCustomerCreditCard(), referenceRes.getCustomerName(), "1234 1234 1234 1234", "Ophelia's Oasis", referenceRes.GetTotalPrice());
                referenceRes.SetPaid(true);
            }
            printAccomodationBill();
            referenceRes.checkOut();
            Htl.clearRoom(referenceRes.getRoomNumber());
            
            Console.WriteLine("Check-out successful. Accomodation bill printed.");
            return "";
        }

        /// <summary>
        /// A method that builds the accomodation bill and stores it in a file.
        /// </summary>
        private static void printAccomodationBill()
        {
            List<String> accomBill = new List<String>();
            DateTime today = DateTime.Today;
            int nights = referenceRes.getEndDate().Subtract(referenceRes.getStartDate()).Days;
            accomBill.Add(today.ToShortDateString());
            accomBill.Add(referenceRes.getCustomerName());
            accomBill.Add("Room Number: " + referenceRes.getRoomNumber());
            accomBill.Add("Arrival Date: " + referenceRes.getStartDate().ToShortDateString());
            accomBill.Add("Departure Date: " + referenceRes.getEndDate().ToShortDateString());
            accomBill.Add("Nights: " + nights);
            accomBill.Add("Total Charge: " + referenceRes.GetTotalPrice());
            if (referenceRes.getReservationType().Equals(ReservationType.Prepaid) || referenceRes.getReservationType().Equals(ReservationType.SixtyDay))
            {
                accomBill.Add("Payment Date: " + referenceRes.getPaymentDate() + ", " + referenceRes.GetTotalPrice());
            }

            String file = @"C:\OpheliasOasis\AccomodationBills\" + today.ToString("dd-MM-yyyy") + " " + referenceRes.getCustomerName() + ".txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < accomBill.Count; i++)
                {
                    sw.WriteLine(accomBill[i]);
                }
            }
        }
    }
}
