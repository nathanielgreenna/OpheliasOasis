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
    class Program
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
            ReservationPageHandler.Init(new ReservationDB(), calendar);
            RecordsPageHandler.Init(reservationDB, calendar, hotel ,managerPassword);

            //level 2 of tree





            // level 1 of tree
            //ProcessPage dates = new ProcessPage("Dates", "Dates", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            //ProcessPage reportsEmailsBackups = new ProcessPage("Reports, Emails, and Backups", "Reports, Emails, and Backups", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            //MenuPage reservations = new MenuPage("Reservation Menu", "Place, update, or cancel a reservation", new List<Page> { ReservationPageHandler.p, ReservationPageHandler.u, ReservationPageHandler.c });

            //Home. this is top of the tree
            //MenuPage home = new MenuPage("Home Menu", "Ophelia's Oasis Home Menu", new List<Page> { checkIn, checkOut, dates, reportsEmailsBackups, reservations });

            while(true)
            {
                System.Threading.Thread.Sleep(2000);
                RecordsPageHandler.recordsMenu.Open();
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
            Console.WriteLine("Loading most recent backup...");


            Directory.CreateDirectory("C:\\OpheliasOasis");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Archive");
            Directory.CreateDirectory("C:\\OpheliasOasis\\Reports");
            Directory.CreateDirectory("C:\\OpheliasOasis\\EmailCCStubs");


            XMLformat g;
            DateTime t = DateTime.Today;
            for (int i = 0; i < 100; i++) 
            {
                try 
                {
                    g = XMLreader.XMLin(t);
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write("Loaded Most Recent Backup From " + t.ToString("D"));
                    reservationDB = g.R;
                    calendar = g.C;
                    hotel = g.H;
                    managerPassword = g.M;
                    System.Threading.Thread.Sleep(2000);
                    return;

                }
                catch(FileNotFoundException)
                {
                    t = t.AddDays(-1);
                }

            }
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("Backup not found within the last 100 days.");
            String inp;
            Console.Write("Type SETUP if this is the first time opening the application, or enter a date in DD/MM/YYYY format to attempt to load: ");

            while (true)
            { 
                inp = Console.ReadLine();
                switch (inp) 
                {
                    case "SETUP":
                        reservationDB = new ReservationDB();
                        hotel = new Hotel();
                        calendar = new Calendar();
                        Console.WriteLine("Welcome to the application!");
                        System.Threading.Thread.Sleep(2000);
                        SetManagerPassword();
                        return;
                    default:
                        if (DateTime.TryParse(inp, out t))
                        {
                            if (File.Exists(@".\" + t.ToString("D")))
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
            System.Threading.Thread.Sleep(4000);

        }












    }
}
