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
        private static Reservation newResTemp;
        private static Reservation? oldResTemp = null;
        private static List<Reservation> tempReses;
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

            // Initialize menu options
            placeRes = new ProcessPage("Place Reservation", "Place a new reservation",
                new List<String> {
                    "Enter the start date (determines the reservation types availble)",
                    "Enter the end date",
                    "Enter the desired reservation type (1: Conventional, 2: Incentive, 3: 60-days, 4: Prepaid)",
                    "Enter guest name",
                    "Enter credit card number (you may press <enter> to skip for 60-days reservations)",
                    "Enter email (you may press <enter> to skip unless you skipped Step 5)"
                },
                new List<Func<String, String>> {
                    InputStartDate,
                    InputEndDate,
                    InputType,
                    InputName,
                    InputCreditCard,
                    InputEmail
                }, SaveTempToDB);

            updateRes = new ProcessPage("Update Reservation", "Update an existing reservation",
                new List<String> {
                },
                new List<Func<String, String>> {
                }, Test);

            cancelRes = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);

            // Initialize menu
            resMenu = new MenuPage("Reservations", "Reservations submenu (place, update, or cancel a reservation)", new List<Page> { placeRes, updateRes, cancelRes });
            /*
            p = new ProcessPage("Place Reservation", "Place a new reservation", ReservationPageHandler.addResPrompts, ReservationPageHandler.addResFunctions, ReservationPageHandler.addRestoDB);
            u = new ProcessPage("Update Reservation", "Update an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            c = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            */
        }

        /// <summary>
        /// Search for reservations under the name specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputSearchName(String input)
        {
            if (!input.Contains(" ")) return "First and last name required";

            tempReses = rdb.getReservation(input);

            if (tempReses.Count < 1) return $"No reservations under the name \"{input}\"";

            Console.WriteLine($"The reservations for {input} are as follows:");
            for (int i = 0; i < tempReses.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {tempReses[i].getStartDate().ToShortDateString()} to {tempReses[i].getEndDate().ToShortDateString()} ({tempReses[i].getReservationStatus()})");
            }

            return "";
        }

        /// <summary>
        /// Select the reservation based on the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputResSelection(String input)
        {
            int selection;
            if (!int.TryParse(input, out selection) || selection > tempReses.Count || selection < 1)
            {
                return $"\"{input}\" is not between 1 and {tempReses.Count}";
            }

            oldResTemp = tempReses[selection];

            return "";
        }

        /// <summary>
        /// Start a new reservation on the date specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputStartDate(String input)
        {
            DateTime startDate;

            // Intialize new reservation
            newResTemp = new Reservation();

            // Import start date from original, when required/possible
            if (String.IsNullOrEmpty(input) && oldResTemp != null)
            {
                newResTemp.setStartDate(oldResTemp.getStartDate());
                return "";
            }

            // Validate date
            if (!DateTime.TryParse(input, out startDate)) return $"\"{input}\" is not a valid date";
            if (startDate < DateTime.Today) return $"{startDate.ToShortDateString()} is before today ({DateTime.Today.ToShortDateString()})";
            if (cal.retrieveDate(startDate).IsFull()) return $"Hotel is full on {startDate.ToShortDateString()}";

            // Save start date
            newResTemp.setStartDate(startDate);
            return "";
        }

        /// <summary>
        /// Set the start date of the new reservation to the date specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputEndDate(String input)
        {
            DateTime endDate;

            // Extrapolate end date from original, when required/possible
            if (String.IsNullOrEmpty(input) && oldResTemp != null)
            {
                // By default, keep the reservation the same length
                newResTemp.setEndDate(oldResTemp.getEndDate().AddDays((newResTemp.getStartDate() - oldResTemp.getStartDate()).TotalDays));
                return "";
            }

            // Check availibility
            Console.Write("Confirming availibility... ");

            // Validate end date
            if (!DateTime.TryParse(input, out endDate)) return $"\"{input}\" is not a valid date";
            if (endDate < newResTemp.getStartDate()) return $"{endDate.ToShortDateString()} is before start date ({newResTemp.getStartDate().ToShortDateString()})";
            if (cal.retrieveDate(endDate).IsFull()) return $"Hotel is full on {endDate.ToShortDateString()}";

            // Ensure space is availible on all intermediate days
            for (DateTime d = newResTemp.getStartDate(); d < endDate; d = d.AddDays(1))
            {
                if (cal.retrieveDate(d).IsFull()) return $"Hotel is full on {d.ToShortDateString()}";
            }

            // Confirm availibility
            Console.WriteLine("Availibility confirmed.");

            // Save end date
            newResTemp.setEndDate(endDate);
            return "";
        }

        /// <summary>
        /// Set the reservation type from the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputType(String input)
        {
            ReservationType type = ReservationType.Conventional;
            int daysAdvance = (int) (newResTemp.getStartDate() - DateTime.Today).TotalDays;
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
                    for (DateTime d = newResTemp.getStartDate(); d <= newResTemp.getEndDate(); d = d.AddDays(1)) totalOccupancy += cal.retrieveDate(d).getOccupancy();
                    totalOccupancyPercent = (double) totalOccupancy / (1.0 + (newResTemp.getEndDate() - newResTemp.getStartDate()).TotalDays);
                    if (totalOccupancyPercent > 0.6) return $"Incentive reservations are only allowed when the avergage occupancy is under 60% (currently {100*totalOccupancy}%)";

                    // Accept otherwise
                    discount = 0.20;
                    break;

                // Sixty Day
                case "3":
                    if (daysAdvance < 60) return $"60 days advance reservations can only be made 60 days in advance, not {daysAdvance} days";
                    discount = 0.15;
                    break;

                // Prepaid
                case "4":
                    if (daysAdvance < 90) return $"Prepaid reservations can only be made 90 days in advance, not {daysAdvance} days";
                    discount = 0.25;
                    break;

                default: return $"{input} is not between 1 and 4";
            }

            // Calculate the price
            Console.Write("Calculating price... ");
            double cost = 0;
            for (DateTime d = newResTemp.getStartDate(); d <= newResTemp.getEndDate(); d = d.AddDays(1)) cost += cal.retrieveDate(d).getBasePrice();
            cost *= (1.0 - discount);
            newResTemp.setTotalPrice(cost);

            // Display the price
            if (discount > 0) Console.Write($"Applied {discount:P} discount. ");
            Console.WriteLine($"Calculated final price: {cost:C2}.");

            // Set the type if no issues are encountered
            newResTemp.setReservationType(type);
            return "";
        }

        /// <summary>
        /// Set the name for the reservation to the name specified in the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputName(String input)
        {
            // Check for missing first or last name
            if (!input.Contains(" "))
            {
                // Return error if name is required
                if (String.IsNullOrEmpty(input) && !String.IsNullOrEmpty(newResTemp.getCustomerName())) return "";
                else return "First and last name required";
            }

            newResTemp.setCustomerName(input);
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

            // Return if none is provided and non is required
            if (newResTemp.getReservationType() == ReservationType.SixtyDay || (sets.Length == 0 && newResTemp.getCustomerCreditCard() != "")) return "";

            // Otherwise, return an error if the number isnt 4 sets of 4 numbers
            if (sets.Length != 4) return "Format must be XXXX XXXX XXXX XXXX";

            for (int i = 0; i < 4; i ++)
            {
                if (sets[i].Length != 4) return "Format must be XXXX XXXX XXXX XXXX";

                for (int j = 0; j < 4; j++)
                {
                    if (!Char.IsDigit(sets[i][j])) return $"\"{sets[i][j]}\" is not a number";
                }
            }

            // If payment is required immediately

            // Save the credit card information if everything checks out
            newResTemp.setCustomerCreditCard(input);
            return "";
        }

        /// <summary>
        /// Set the customer email for reservation from the user's response to the prompt.
        /// </summary>
        /// <param name="input">A string containing the user's reponse to the prompt.</param>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String InputEmail(String input)
        {
            // Check for blatantly bogus email
            if (!input.Contains("@") || !input.Contains("."))
            {
                // Return error message if email is required
                if (String.IsNullOrEmpty(input) && (newResTemp.getReservationType() != ReservationType.SixtyDay || !String.IsNullOrEmpty(newResTemp.getCustomerCreditCard()))) return "";
                else return $"\"{input}\" is not a vald email address";
            }

            newResTemp.setCustomerEmail(input);
            return "";
        }

        /// <summary>
        /// Save the temporary reservation to the database.
        /// </summary>
        /// <returns>A string containing any error message if applicable. A blank string otherwise.</returns>
        static String SaveTempToDB()
        {
            rdb.addReservation(newResTemp);
            return "";
        }

        //
        // vv OLD METHODS vv
        //
        /*
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
        }*/

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
