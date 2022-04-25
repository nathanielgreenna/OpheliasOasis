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
        private static Reservation d;
        private static ProcessPage checkOut;

        public static void Init(ReservationDB db, Hotel htl)
        {
            // Initialize references
            ResDB = db;
            Htl = htl;

            // Initialize page
            checkOut = new ProcessPage("Check Out", "Check Out", new List<Tuple<Func<String, String>, String>>{ Tuple.Create<Func<String, String>, String>(CheckOutRes, "Input Name")}, null);
        }

        /// <summary>
        /// A method that returns the check-out page.
        /// </summary>
        /// <returns>The check-out page.</returns>
        public static ProcessPage getPage()
        {
            return checkOut;
        }

        /// <summary>
        /// A method that finds the reservation for a customer, and if found, checks them out.
        /// </summary>
        /// <param name="input">A string containing the input.</param>
        /// <returns>A string containing the reason why the input is not valid, if applicable. Otherwise a success message.</returns>
        public static String CheckOutRes(String inStr)
        {
            List<Reservation> reservations = ResDB.getReservation(inStr);
            if (reservations == null)
            {
                return "No reservations found for \"" + inStr + "\".";
            }
            else
            {
                DateTime today = DateTime.Today;
                d = null;
                foreach (Reservation reservation in reservations)
                {
                    if (reservation.getStartDate().DayOfYear == today.DayOfYear)
                    {
                        d = reservation; break;
                    }
                }
                if (d == null)
                {
                    return "No reservations for \"" + inStr + "\" end today.";
                }
                else
                {
                    d.checkOut();
                    Htl.clearRoom(d.getRoomNumber());
                    printAccomodationBill();
                    return "Check-out successful. Accomodation bill printed.";
                }
            }
        }

        /// <summary>
        /// A method that builds the accomodation bill and stores it in a file.
        /// </summary>
        private static void printAccomodationBill()
        {
            List<String> accomBill = new List<String>();
            DateTime today = DateTime.Today;
            int nights = d.getEndDate().Subtract(d.getStartDate()).Days;
            accomBill.Add(today.ToShortDateString());
            accomBill.Add(d.getCustomerName());
            accomBill.Add("Room Number: " + d.getRoomNumber());
            accomBill.Add("Arrival Date: " + d.getStartDate().ToShortDateString());
            accomBill.Add("Departure Date: " + d.getEndDate().ToShortDateString());
            accomBill.Add("Nights: " + nights);
            accomBill.Add("Total Charge: " + d.getTotalPrice());
            if (d.getReservationType().Equals(ReservationType.Prepaid) || d.getReservationType().Equals(ReservationType.SixtyDay))
            {
                accomBill.Add("Payment Date: " + d.getPaymentDate() + ", " + d.getTotalPrice());
            }

            String file = @"C:\OpheliasOasis\AccomodationBills\" + today.ToString("dd-MM-yyyy") + @"\" + d.getCustomerName() + ".txt";
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
