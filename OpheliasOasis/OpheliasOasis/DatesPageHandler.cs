using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{

    // view prices, view one date, change one price, setting unset prices for the year ahead.
    class DatesPageHandler
    {
        private static Calendar cal;
        private static String manPass;

        // Dates submenu
        public static MenuPage datesMenu;

        public static ProcessPage setForYear;
        public static ProcessPage setIndividual;
        public static ProcessPage showIndividual;
        public static ProcessPage showYearAhead;

        private static List<Tuple<Func<String, String>, String>> yearOfSetting;


        private readonly static Tuple<Func<String, String>, String> checkPS =
        Tuple.Create<Func<String, String>, String>(checkPasschangeSingle, "Input Manager Password");
        private readonly static Tuple<Func<String, String>, String> recieveDate =
        Tuple.Create<Func<String, String>, String>(getInDate, "Input Date to have price changed (DD/MM/YYYY)");
        private readonly static Tuple<Func<String, String>, String> recievePrice =
        Tuple.Create<Func<String, String>, String>(getInPrice, "Input new base price");
        private readonly static Tuple<Func<String, String>, String> viewDate =
        Tuple.Create<Func<String, String>, String>(getInDate, "Input Date (DD/MM/YYYY)");
        private readonly static Tuple<Func<String, String>, String> viewYear =
        Tuple.Create<Func<String, String>, String>(showYearOfDates, "This will print a year's worth of dates and their prices");

        public static void Init(Calendar cl, String mPass)
        {
            cal = cl;
            manPass = mPass;
            yearOfSetting = new List<Tuple<Func<String, String>, String>>();
            yearOfSetting.Add(new Tuple<Func<String, String>, String>(checkPasschangeYear, "Input Manager Password"));
            for (int i = 0; i < 365; i++) 
            {
                yearOfSetting.Add(new Tuple<Func<String, String>, String>(inputDatePrice, "Input New Price, <Enter> to skip"));
            }







            showYearAhead = new ProcessPage("Date Information for Next Year", "Display a year of dates and prices", new List<Tuple<Func<String, String>, String>> { viewYear }, null);

            showIndividual = new ProcessPage("Individual Date Information", "Display a specific date", new List<Tuple<Func<String, String>, String>> { viewDate }, null);

            setIndividual = new ProcessPage("Set Specific Date Price (Manager Only)", "Sets a specific date", new List<Tuple<Func<String, String>, String>> { checkPS, recieveDate, recievePrice }, setInPrice);

            setForYear = new ProcessPage("Set Prices The Year Ahead (Manager Only)", "Set a price or skip for each day in the year ahead", yearOfSetting, updateYearPrices);


            datesMenu = new MenuPage("Dates", "Dates submenu", new List<Page> { showIndividual, showYearAhead, setIndividual , setForYear});
        }






        public static void setPassword(String mPass)
        {
            manPass = mPass;
        }





        //SET DATES FOR THE YEAR AHEAD
        private static double[] g = new double[365];
        private static int currDay;

        static String checkPasschangeYear(String input)
        {
            if (input != manPass)
            {
                return "Password incorrect.";
            }
            currDay = 1;
            Console.Write("Current Price for " + DateTime.Today.AddDays(currDay).ToString("ddd dd-MM-yyyy") + ": " + cal.retrieveDate(DateTime.Today.AddDays(currDay)).getBasePrice());
            return "";


        }

        static String inputDatePrice(String input)
        {

            if (String.IsNullOrEmpty(input)) 
            {
                g[currDay - 1] = -1;
                currDay += 1;
                Console.Write("Current Price for " + DateTime.Today.AddDays(currDay).ToString("ddd dd-MM-yyyy") + ": " + cal.retrieveDate(DateTime.Today.AddDays(currDay)).getBasePrice());
                return ""; 
            }
            if (double.TryParse(input, out double inPrice))
            {
                g[currDay - 1] = inPrice;
                currDay += 1;
                Console.Write("Current Price for " + DateTime.Today.AddDays(currDay).ToString("ddd dd-MM-yyyy") + ": " + cal.retrieveDate(DateTime.Today.AddDays(currDay)).getBasePrice());
                return "";
            }
            else
            {
                return "not a valid price";
            }
        }

        static String updateYearPrices()
        {
            for(int i = 1; i < 366; i++)
            {
                if (g[i - 1] != -1)
                {
                    cal.retrieveDate(DateTime.Today.AddDays(i)).setBasePrice(g[i - 1]);
                }
            }

            return "";
        }

        //Update a single price
        private static DateTime dateToChange;
        private static double inPrice;
        static String checkPasschangeSingle(String input)
        {
            if (input != manPass)
            {
                return "Password incorrect.";
            }
            return "";
        }

        static String getInDate(String input)
        {
            if (DateTime.TryParse(input, out dateToChange))
            {
                Console.Write("Current Price: " + cal.retrieveDate(dateToChange).getBasePrice() + " Current Occupancy: " + cal.retrieveDate(dateToChange).getOccupancy());
                return ("");
            }
            else
            {
                return ("Not the correct format.");
            }
        }

        static String getInPrice(String input)
        {
            if (double.TryParse(input, out inPrice))
            {
                return ("");
            }
            else
            {
                return ("Not a valid date");
            }
        }

        static String setInPrice()
        {
            cal.retrieveDate(dateToChange).setBasePrice(inPrice);
            return "";
        }

        //show a year of prices
        static String showYearOfDates(String input)
        {
            for(int i = 0; i < 365; i++)
            {

                Console.Write(DateTime.Today.AddDays(i).ToString("ddd dd-MM-yyyy") + ": " + cal.retrieveDate(DateTime.Today.AddDays(i)).getBasePrice() + "          ");

                if (i % 7 == 0) { Console.WriteLine(""); }
            }
            return "";
        }















    }


}
