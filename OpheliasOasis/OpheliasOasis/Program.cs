/*
 * Program
 * 
 * Description: The controlling class for the program. Loads the database and allows
 * the user to manipulate it. See methodss for more details.
 * 
 * Changelog:
 */

using System;
using System.Collections.Generic;
using System.IO;


namespace OpheliasOasis
{
    /// <summary>
    /// The controlling class for Ophelia's Oasis Reservation System
    /// </summary>
    public class Program
    {
        private static readonly int MAX_OCCUPANCY = 45;
        private static ReservationDB reservationDB;
        private static Calendar calendar;
        private static Hotel hotel;
        private static String managerPassword;

        static void Main(string[] args)
        {
            //Ophelia's is in AU, so make culture AU
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-AU");

            StartupScreen();
            SetupExpectedOccupancyReportTest();
            CheckInPageHandler.Init(reservationDB, hotel);
            CheckOutPageHandler.Init(reservationDB, hotel);
            DatesPageHandler.Init(calendar, managerPassword);
            RecordsPageHandler.Init(reservationDB, calendar, hotel, managerPassword);
            ReservationPageHandler.Init(reservationDB, calendar);

            //level 2 of tree





            // level 1 of tree
            //ProcessPage dates = new ProcessPage("Dates", "Dates", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            //ProcessPage reportsEmailsBackups = new ProcessPage("Reports, Emails, and Backups", "Reports, Emails, and Backups", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            //MenuPage reservations = new MenuPage("Reservation Menu", "Place, update, or cancel a reservation", new List<Page> { ReservationPageHandler.p, ReservationPageHandler.u, ReservationPageHandler.c });

            //Home. this is top of the tree
            MenuPage home = new MenuPage("Home Menu", "Ophelia's Oasis Home Menu", new List<Page> { CheckInPageHandler.getPage(), CheckOutPageHandler.getPage(), DatesPageHandler.datesMenu, RecordsPageHandler.recordsMenu, ReservationPageHandler.resMenu });
            System.Threading.Thread.Sleep(2000);
            String exitInput;
            while (true)
            {
                home.Open();
                Console.Write("Do you really want to exit? type YES to quit: ");
                exitInput = Console.ReadLine();
                if(exitInput == "YES") 
                {
                    Environment.Exit(0);
                }
            }

        }

        /// <summary>
        /// Walk the user through updating a reservation.
        /// </summary>
        static String Placeholder(String inStr)
        {
            Console.WriteLine("Place successfully held!");
            return "";
        }

        /// <summary>
        /// Walk the user through cancelling a reservation.
        /// </summary>
        static String CancelReservation(String inStr)
        {
            Console.WriteLine("Cancel");
            return "";
        }

        /// <summary>
        /// Clear the screen and display a header for the new screen.
        /// </summary>
        /// <param name="header"></param>
        static void DisplayHeader(string header)
        {
            Console.Clear();
            Console.WriteLine("====================| " + header.ToUpper() + " |====================");
            Console.WriteLine();
        }

        /// <summary>
        /// Display a reminder for how to navigate multi-step processes.
        /// </summary>
        static void DisplayStepNavigationHint()
        {
            Console.WriteLine("Special input commands:");
            Console.WriteLine("\tB: Move back a step");
            Console.WriteLine("\tQ: Quit this screen");
            Console.WriteLine();
        }

        /// <summary>
        /// Startup code
        /// </summary>
        static void StartupScreen()
        {
            Console.WriteLine("    .    _    +     .  ______   .          .  ");
            Console.WriteLine(" (      /|\\      .    |      \\      .   +   ");
            Console.WriteLine("     . |||||     _    | |   | | ||         .  ");
            Console.WriteLine(".      |||||    | |  _| | | | |_||    .       ");
            Console.WriteLine("   /\\  ||||| .  | | |   | |      |       .   ");
            Console.WriteLine("__||||_|||||____| |_|_____________\\__________");
            Console.WriteLine(". |||| |||||  /\\   _____      _____  .   .   ");
            Console.WriteLine("  |||| ||||| ||||   .   .  .         ________ ");
            Console.WriteLine(" . \\|`-'|||| ||||    __________       .    . ");
            Console.WriteLine("    \\__ |||| |||| Ophelia's Oasis  .    .    ");
            Console.WriteLine(" __    ||||`-'|||  .       .    __________    ");
            Console.WriteLine(".    . |||| ___/  ___________             .   ");
            Console.WriteLine("   . _ ||||| . _               .   _________  ");
            Console.WriteLine("_   ___|||||__  _    .   . _____   .         _");
            Console.WriteLine("     _ `---'    .    ..   _   .   .  ____    .");
            Console.WriteLine("Loading...");
            //Credit to https://www.asciiart.eu/nature/deserts for the text art.

            Directory.CreateDirectory("C:\\OpheliasOasis");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Backups");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Reports");
            Directory.CreateDirectory("C:\\OpheliasOasis\\EmailCCStubs");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Data\\Reservations");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Data\\Calendar");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Data\\Hotel");
            Directory.CreateDirectory("C:\\OpheliasOasis\\AccomodationBills");



            try 
            {
                managerPassword = XMLreader.readInMPass();
                hotel = XMLreader.readInHotel();
                reservationDB = XMLreader.readInResDB();
                calendar = XMLreader.readInCal();
                return;
            }
            catch (FileNotFoundException){

            }





            XMLformat g;
            DateTime t = DateTime.Today;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("Previous data was not found or has been corrupted");
            String inp;
            Console.Write("Type SETUP if this is the first time opening the application, or enter a date in DD/MM/YYYY format to attempt to load that date's backup: ");

            while (true)
            { 
                inp = Console.ReadLine();
                switch (inp) 
                {
                    case "SETUP":
                        XMLreader.clearFolders();
                        reservationDB = new ReservationDB();
                        hotel = new Hotel(MAX_OCCUPANCY);
                        calendar = new Calendar();
                        XMLreader.changeHotel(hotel);
                        Console.WriteLine("Welcome to the application!");
                        System.Threading.Thread.Sleep(2000);
                        SetManagerPassword();
                        return;
                    default:
                        if (DateTime.TryParse(inp, out t))
                        {
                            if (File.Exists("C:\\OpheliasOasis\\Backups\\" + t.ToString("D")))
                            {
                                g = XMLreader.XMLin(t);
                                reservationDB = g.R;
                                hotel = g.H;
                                calendar = g.C;
                                managerPassword = g.M;
                                Console.WriteLine("File found! Loading...");
                                System.Threading.Thread.Sleep(2000);
                                return;
                            }
                            else
                            {
                                Console.Write("\nNo archive found for  \"" + inp + "\". ");
                                break;
                            }
                        }
                        else
                        {
                            Console.Write("\n\"" + inp + "\" Isn't a valid date. ");
                            break;
                        }
                }
                Console.Write("Try again: ");

            }

            






        }


        static void SetManagerPassword()
        {
            String candidatePassword = "";
            String confirmPassword = "";
            while (candidatePassword.Length < 5) 
            {
                Console.Write("Enter your new manager password. Must be at least 5 characters: ");
                candidatePassword = Console.ReadLine();
            }
            while (confirmPassword != candidatePassword)
            {
                Console.Write("Confirm manager password: ");
                confirmPassword = Console.ReadLine();
            }
            managerPassword = candidatePassword;
            Console.WriteLine("Manager password set! This can be changed later in the Records menu.");
            XMLreader.changeMPass(managerPassword);
            System.Threading.Thread.Sleep(4000);

        }


        public static void setPassword(String newpass) 
        {
            managerPassword = newpass;
            XMLreader.changeMPass(managerPassword);
        }

        private static void SetupArrivalsReportTest()
        {
            Console.WriteLine("Testing Arrival Report...");
            System.Threading.Thread.Sleep(50);

            reservationDB = new ReservationDB();
            DateTime twoDaysAgo = DateTime.Today.AddDays(-2);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime twoDaysFromNow = DateTime.Today.AddDays(2);

            List<Tuple<DateTime, DateTime, ReservationStatus, int>> rinfo = new List<Tuple<DateTime, DateTime, ReservationStatus, int>>
            {
                Tuple.Create(twoDaysAgo, twoDaysFromNow, ReservationStatus.CheckedIn, 7),
                Tuple.Create(twoDaysAgo, yesterday, ReservationStatus.CheckedOut, 1),
                Tuple.Create(twoDaysAgo, today, ReservationStatus.CheckedIn, 2),
                Tuple.Create(today, tomorrow, ReservationStatus.Confirmed, 5),
                Tuple.Create(today, tomorrow, ReservationStatus.Cancelled, 3),
                Tuple.Create(today, tomorrow, ReservationStatus.CheckedOut, 4),
                Tuple.Create(tomorrow, twoDaysFromNow, ReservationStatus.Confirmed, 6)
            };

            for (int i = 0; i < rinfo.Count; i++)
            {
                int j = rinfo[i].Item4;
                Reservation res = new Reservation(ReservationType.Conventional, "Guest " + j, $"{j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j}", rinfo[i].Item1, rinfo[i].Item2);
                res.setReservationStatus(rinfo[i].Item3);
                res.setRoomNumber(rinfo.Count + 1 - j);
                reservationDB.addReservation(res);
            }
        }

        private static void SetupOccupancyReportTest()
        {
            Console.WriteLine("Testing Occupancy Report...");
            System.Threading.Thread.Sleep(50);

            reservationDB = new ReservationDB();
            DateTime twoDaysAgo = DateTime.Today.AddDays(-2);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime twoDaysFromNow = DateTime.Today.AddDays(2);

            List<Tuple<DateTime, DateTime, ReservationStatus, int>> rinfo = new List<Tuple<DateTime, DateTime, ReservationStatus, int>>
            {
                Tuple.Create(twoDaysAgo, twoDaysFromNow, ReservationStatus.CheckedIn, 7),
                Tuple.Create(twoDaysAgo, yesterday, ReservationStatus.CheckedOut, 1),
                Tuple.Create(yesterday, today, ReservationStatus.Cancelled, 2),
                Tuple.Create(yesterday, today, ReservationStatus.CheckedOut, 3),
                Tuple.Create(yesterday, today, ReservationStatus.CheckedIn, 4),
                Tuple.Create(today, tomorrow, ReservationStatus.CheckedOut, 5),
                Tuple.Create(tomorrow, twoDaysFromNow, ReservationStatus.Confirmed, 6)
            };

            for (int i = 0; i < rinfo.Count; i++)
            {
                int j = rinfo[i].Item4;
                Reservation res = new Reservation(ReservationType.Conventional, "Guest " + j, $"{j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j}", rinfo[i].Item1, rinfo[i].Item2);
                res.setReservationStatus(rinfo[i].Item3);
                res.setRoomNumber(rinfo.Count + 1 - j);
                reservationDB.addReservation(res);
            }
        }

        private static void SetupChargeNoShowsTest()
        {
            Console.WriteLine("Testing No Show Charges...");
            System.Threading.Thread.Sleep(50);

            reservationDB = new ReservationDB();
            DateTime twoDaysAgo = DateTime.Today.AddDays(-2);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime twoDaysFromNow = DateTime.Today.AddDays(2);

            List<Tuple<DateTime, DateTime, ReservationStatus, int>> rinfo = new List<Tuple<DateTime, DateTime, ReservationStatus, int>>
            {
                Tuple.Create(yesterday, today, ReservationStatus.CheckedIn, 1),
                Tuple.Create(yesterday, today, ReservationStatus.Confirmed, 2),
                Tuple.Create(yesterday, today, ReservationStatus.Cancelled, 3),
                Tuple.Create(twoDaysAgo, yesterday, ReservationStatus.CheckedOut, 4)
            };

            for (int i = 0; i < rinfo.Count; i++)
            {
                int j = rinfo[i].Item4;
                Reservation res = new Reservation(ReservationType.Conventional, "Guest " + j, $"{j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j}", rinfo[i].Item1, rinfo[i].Item2);
                res.setReservationStatus(rinfo[i].Item3);
                res.setRoomNumber(rinfo.Count + 1 - j);
                res.SetPrices(new List<double> { 20 });
                reservationDB.addReservation(res);
            }
        }

        private static void SetupExpectedOccupancyReportTest()
        {
            Console.WriteLine("Testing Expected Occupancy Report...");
            System.Threading.Thread.Sleep(50);

            reservationDB = new ReservationDB();
            DateTime twoDaysAgo = DateTime.Today.AddDays(-2);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);
            DateTime preEnd30 = DateTime.Today.AddDays(28);
            DateTime end30 = DateTime.Today.AddDays(29);
            DateTime postEnd30 = DateTime.Today.AddDays(30);
            DateTime postEnd302 = DateTime.Today.AddDays(31);

            List<Tuple<DateTime, DateTime, ReservationStatus, int>> rinfo = new List<Tuple<DateTime, DateTime, ReservationStatus, int>>
            {
                Tuple.Create(twoDaysAgo, yesterday, ReservationStatus.CheckedOut, 1),
                Tuple.Create(today, tomorrow, ReservationStatus.CheckedIn, 2),
                Tuple.Create(tomorrow, end30, ReservationStatus.Confirmed, 3),
                Tuple.Create(end30, postEnd30, ReservationStatus.Confirmed, 4),
                Tuple.Create(postEnd30, postEnd302, ReservationStatus.Confirmed, 5),
                Tuple.Create(yesterday, postEnd30, ReservationStatus.Confirmed, 6),
                Tuple.Create(yesterday, postEnd30, ReservationStatus.Cancelled, 7),
            };

            for (int i = 0; i < rinfo.Count; i++)
            {
                int j = rinfo[i].Item4;
                Reservation res = new Reservation(i == 2 ? ReservationType.Incentive : ReservationType.Conventional, "Guest " + j, $"{j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j} {j}{j}{j}{j}", rinfo[i].Item1, rinfo[i].Item2);
                res.setReservationStatus(rinfo[i].Item3);
                reservationDB.addReservation(res);
            }
        }




    }
}
