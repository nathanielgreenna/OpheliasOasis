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
        private static Reservation d;
        private static ProcessPage checkIn;

        public static List<Func<String, String>> checkInFunctions = new List<Func<String, String>> { CheckInRes };

        public static void Init(ReservationDB db, Hotel htl)
        {
            // Initialize references
            ResDB = db;
            Htl = htl;

            // Initialize page
            checkIn = new ProcessPage("Check In", "Check In", new List<String> { "Input Name" }, checkInFunctions, null);
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
        static String CheckInRes(String input)
        {
            List<Reservation> reservations = ResDB.getReservation(input);
            if (reservations == null)
            {
                return "No reservations found for \"" + input + "\".";
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
                    return "No reservations for \"" + input + "\" start today.";
                }
                else
                {
                    d.checkIn();
                    int roomNum = Htl.assignRoom();
                    d.setRoomNumber(roomNum);
                    return "Check-in successful. Room Number is " + roomNum + ".";
                }
            }
        }
    }
}
