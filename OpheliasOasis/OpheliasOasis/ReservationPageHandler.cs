/*
 * ReservationPageHandler
 * 
 * Description: A class to store methods and pages related to Reservations.
 * 
 * Changelog:
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    public static class ReservationPageHandler
    {
        private static Calendar cal;
        private static ReservationDB rdb;
        private static Reservation tempRes;
        /*
        public static ProcessPage p;
        public static ProcessPage u;
        public static ProcessPage c;

        public static List<String> addResPrompts = new List<String> { "Input Reservation Type (1 = Conventional, 2 = Incentive, 3 = Sixty Day, 4 = Prepaid)", "Input Customer Name","Input Credit (<Enter> to skip for 60-day)","Input Email (<Enter> to skip if not 60-day)","Input Start Date (DD/MM/YYYY)", "Input End Date (DD/MM/YYYY)" };
        public static List<Func<String, String>> addResFunctions = new List<Func<String, String>> { inputResType, addResName, addResCC, addResEmail, addResStartDate, addResEndDate };
        */
        // Reservation submenu
        public static MenuPage resMenu;

        // Menu options
        public static ProcessPage placeRes;
        public static ProcessPage updateRes;
        public static ProcessPage cancelRes;

        public static void Init(ReservationDB db, Calendar cl)
        {
            // Initialize references
            rdb = db;
            cal = cl;

            // Initialize menu
            resMenu = new MenuPage("Reservations", "Reservations submenu (place, update, or cancel a reservation)", new List<Page> { placeRes, updateRes, cancelRes });

            // Initialize menu options
            placeRes = new ProcessPage("Place Reservation", "Place a new reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);
            updateRes = new ProcessPage("Update Reservation", "Update an existing reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);
            cancelRes = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);
            /*
            p = new ProcessPage("Place Reservation", "Place a new reservation", ReservationPageHandler.addResPrompts, ReservationPageHandler.addResFunctions, ReservationPageHandler.addRestoDB);
            u = new ProcessPage("Update Reservation", "Update an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            c = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            */
    }

        /// <summary>
        /// A utility method to parse the input string looking for a valid reservation date.
        /// </summary>
        /// <param name="input">A string containing the input.</param>
        /// <param name="date">An out DateTime in which to read the valid date, if it exists.</param>
        /// <returns>A string containing the reason why the input is not valid, if applicable. Empty otherwise.</returns>
        static String GetValidDate(String input, out DateTime date)
        {
            if (!DateTime.TryParse(input, out date)) return "Start date \"" + input + "\" is not a valid date";
            if (date < DateTime.Today) return date.ToShortDateString() + " is before today";
            if (cal.retrieveDate(date).IsFull()) return "Hotel is full on " + date.ToShortDateString();
            return "";
        }

        /// <summary>
        /// Start a new reservation on the date specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns></returns>
        static String InputNewReservationDate(String input)
        {
            DateTime date;
            String dateError = GetValidDate(input, out date);

            if (!String.IsNullOrEmpty(dateError)) return dateError;

            tempRes = new Reservation();
            tempRes.setStartDate(date);
            return "";
        }

        /// <summary>
        /// Start a new reservation of the type specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns></returns>
        static String InputNewReservationType(String input)
        {
            switch (input)
            {
                case "1":
                    tempRes.setReservationType(ReservationType.Conventional);
                    return "";
                case "2":
                    tempRes.setReservationType(ReservationType.Incentive);
                    return "";
                case "3":
                    tempRes.setReservationType(ReservationType.SixtyDay);
                    return "";
                case "4":
                    tempRes.setReservationType(ReservationType.Prepaid);
                    return "";
                default:
                    return "\"" + input + "\" is not a valid reservation type";
            }
        }

        //
        // vv OLD METHODS vv
        //

        static String addResName(String inStr)
        {
            if (inStr == null || inStr == "")
            {
                return "A name must be provided";
            }
            d.setCustomerName(inStr);
            return "";
        }

        static String addResCC(String inStr)
        {
            int intCC;
            if (inStr == "" && d.getReservationType() != ReservationType.SixtyDay)
            {
                return "Credit card information is not optional";
            }
            else if (inStr == "")
            {
                d.setCustomerCreditCard(0);
            }
            else if (Int32.TryParse(inStr, out intCC))
            {
                d.setCustomerCreditCard(intCC);
            }
            else
            {
                return "Credit card information must be blank (not provided) or a 9-digit number";
            }

            return "";

        }

        static String addResEmail(String inStr)
        {
            if (inStr == "" && d.getCustomerCreditCard() == 0 && d.getReservationType() == ReservationType.SixtyDay)
            {
                return "Since no credit card information is on file for this 60-day reservation, and email is required";
            }
            else
            {
                d.setCustomerEmail(inStr);
                return "";
            }
        }
        static String addResStartDate(String inStr) // TODO make sure we check to make sure the gotel isnt full
        {
            DateTime t;
            if (DateTime.TryParse(inStr, out t)) 
            {
                if (t > DateTime.Today)
                {
                    d.setStartDate(t);
                    return "";
                }
            }
            return "\"" + inStr + "\" is not a future date";
         }
        static String addResEndDate(String inStr)//TODO we may want this to be a "how long is your stay". Also check to make sure the hotel isnt full on each of these days
        {
            DateTime t;
            if (DateTime.TryParse(inStr, out t))
            {
                if (t > d.getStartDate())
                {
                    d.setEndDate(t);
                    return "";
                }
            }
            return "\"" + inStr + "\" is not a  date after \"" + d.getStartDate().ToString() + "\"";
        }

        public static String addRestoDB() 
        {
            ResDB.addReservation(d);
            return ("No, stop that.");
        }

        static String Placeholder(String inStr)
        {
            Console.WriteLine("Place successfully held!");
            return "";
        }

        private static String Test(String input)
        {
            return "";
        }

        private static String Test()
        {
            return "";
        }
    }
}
