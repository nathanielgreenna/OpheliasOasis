using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    public static class ReservationPageHandler
    {
        private static Reservation d;
        private static ReservationDB ResDB;


        public static List<String> addRes = new List<String> { "Input Reservation Type (1 = Conventional, 2 = Incentive, 3 = Sixty Day, 4 = Prepaid):", "Input Customer Name:","Input Credit (<Enter> to skip for 60-day):","Input Email (<Enter> to skip if not 60-day):","Input Start Date (DD/MM/YYYY): ", "Input End Date (DD/MM/YYYY): " };
        public static List<Func<String, Boolean>> addResFunctions = new List<Func<String, Boolean>> { inputResType, addResName, addResCC, addResEmail, addResStartDate, addResEndDate };




        public static void setReservationDB(ReservationDB db)
        {
            ResDB = db;
        }

        static Boolean inputResType(String inStr)
        {
            d = new Reservation();
            switch (inStr)
            {
                case "1":
                    d.setReservationType(ReservationType.Conventional);
                    return true;
                case "2":
                    d.setReservationType(ReservationType.Incentive);
                    return true;
                case "3":
                    d.setReservationType(ReservationType.SixtyDay);
                    return true;
                case "4":
                    d.setReservationType(ReservationType.Prepaid);
                    return true;
                default:
                    return false;
            }
        }
        static Boolean addResName(String inStr)
        {
            if (inStr == null || inStr == "")
            {
                return false;
            }
            d.setCustomerName(inStr);
            return true;
        }

        static Boolean addResCC(String inStr)
        {
            int intCC;
            if (inStr == "" && d.getReservationType() != ReservationType.SixtyDay)
            {
                return false;
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
                return false;
            }

            return true;

        }

        static Boolean addResEmail(String inStr)
        {
            if (inStr == "" && d.getCustomerCreditCard() == 0 && d.getReservationType() == ReservationType.SixtyDay)
            {
                return false;
            }
            else
            {
                d.setCustomerEmail(inStr);
                return true;
            }
        }
        static Boolean addResStartDate(String inStr)
        {
            DateTime t;
            if (DateTime.TryParse(inStr, out t)) 
            {
                if (t > DateTime.Today)
                {
                    d.setStartDate(t);
                    return true;
                }
            }
            return false;
         }
        static Boolean addResEndDate(String inStr)
        {
            DateTime t;
            if (DateTime.TryParse(inStr, out t))
            {
                if (t > d.getStartDate())
                {
                    d.setEndDate(t);
                    return true;
                }
            }
            return false;
        }

        public static void addRestoDB() 
        {
            ResDB.addReservation(d);
        }



    }
}
