using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    public static class ReservationPageHandler
    {
        private static Reservation d;
        private static ReservationDB ResDB;


        public static List<String> addResPrompts = new List<String> { "Input Reservation Type (1 = Conventional, 2 = Incentive, 3 = Sixty Day, 4 = Prepaid)", "Input Customer Name","Input Credit (<Enter> to skip for 60-day)","Input Email (<Enter> to skip if not 60-day)","Input Start Date (DD/MM/YYYY)", "Input End Date (DD/MM/YYYY)" };
        public static List<Func<String, String>> addResFunctions = new List<Func<String, String>> { inputResType, addResName, addResCC, addResEmail, addResStartDate, addResEndDate };




        public static void setReservationDB(ReservationDB db)
        {
            ResDB = db;
        }

        static String inputResType(String inStr)
        {
            d = new Reservation();
            switch (inStr)
            {
                case "1":
                    d.setReservationType(ReservationType.Conventional);
                    return "";
                case "2":
                    d.setReservationType(ReservationType.Incentive);
                    return "";
                case "3":
                    d.setReservationType(ReservationType.SixtyDay);
                    return "";
                case "4":
                    d.setReservationType(ReservationType.Prepaid);
                    return "";
                default:
                    return "\"" + inStr + "\" is not a valid reservation type";
            }
        }
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



    }
}
