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

namespace OpheliasOasis
{
    /// <summary>
    /// The controlling class for Ophelia's Oasis Reservation System
    /// </summary>
    class Program
    {
        private static readonly int MAX_OCCUPANCY = 45;
        private static readonly ReservationDB reservationDB;
        private static readonly Calendar calendar = new Calendar();

        static void Main(string[] args)
        {
            //Ophelia's is in AU, so make culture AU
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-AU");
            ReservationPageHandler.setReservationDB(new ReservationDB());


            //level 2 of tree
            ProcessPage p = new ProcessPage("Place Reservation", "Place a new reservation", ReservationPageHandler.addResPrompts, ReservationPageHandler.addResFunctions, ReservationPageHandler.addRestoDB);
            ProcessPage u = new ProcessPage("Update Reservation", "Update an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            ProcessPage c = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);




            // level 1 of tree
            ProcessPage checkIn = new ProcessPage("Check In", "Check In", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            ProcessPage checkOut = new ProcessPage("Check Out", "Check Out", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            ProcessPage dates = new ProcessPage("Dates", "Dates", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            ProcessPage reportsEmailsBackups = new ProcessPage("Reports, Emails, and Backups", "Reports, Emails, and Backups", new List<String> { "Input Name" }, new List<Func<String, String>> { Placeholder }, null);
            MenuPage reservations = new MenuPage("Reservation Menu", "Place, update, or cancel a reservation", new List<Page> { p, u, c });

            //Home. this is top of the tree
            MenuPage home = new MenuPage("Home Menu", "Ophelia's Oasis Home Menu", new List<Page> { checkIn, checkOut, dates, reportsEmailsBackups, reservations });

            while(true)
            {
                System.Threading.Thread.Sleep(2000);
                home.Open();
            }

        }

        /// <summary>
        /// Display the menu for adding, creating, or removing reservations.
        /// </summary>
        /*static void ReservationMenu()
        {
            while (true)
            {
                // Display menu options
                DisplayHeader("Reservation Menu");
                Console.WriteLine("Please select one of the following options:");
                Console.WriteLine("\t1: Place a new reservation");
                Console.WriteLine("\t2: Update an existing reservation");
                Console.WriteLine("\t3: Cancel an existing reservation");
                Console.WriteLine("\t4: Return to the main menu");
                Console.WriteLine();

                string selectionText;
                int selection;

                Console.Write("Please enter your selection (1-4): ");
                selectionText = Console.ReadLine();

                // If necessary, repeatedly request a selection until a valid number (1-4) is provided
                while (!int.TryParse(selectionText, out selection) || selection > 4 || selection < 1)
                {
                    Console.Write("Selection \"" + selectionText + "\" is not valid. Please enter your selection (1-4): ");
                    selectionText = Console.ReadLine();
                }

                Console.WriteLine();

                // Navigate to selected process
                switch (selection)
                {
                    //case 1: PlaceReservation(); break;
                   // case 2: UpdateReservation(); break;
                   // case 3: CancelReservation(); break;
                    case 4: return;
                }
            }
        } */

        /// <summary>
        /// Walk the user through placing a reservation.
        /// </summary>
        /*static void PlaceReservation()
        {
            // Store step variables
            int step = 1;
            int maxStep = 5;

            // Store menu variables
            string temp;
            string? name = null;
            int? creditCard = null;
            DateTime? date = null;
            ReservationType? type = null;

            // Display page information
            DisplayHeader("Place New Reservation");
            DisplayStepNavigationHint();

            // Loop through the steps - allows user to move backwards and forwards through the process
            while (step <= maxStep)
            {
                // Keep user informed of progress
                Console.Write("Step " + step + " of " + maxStep + ": ");

                // Perform step
                switch (step)
                {
                    case 1: date = RequestReservationStartDate(date); break;
                }

                // Navigate to the next user-selected step
                Console.Write("Continute? (Enter B, R, C):");

                switch (Console.ReadLine().ToUpper())
                {
                    case "B":
                        if (step > 1)
                        {
                            step--;
                        }
                        else
                        {
                            Console.Write("Are you sure you want to exit this menu? (Y/n):");
                            if (Console.ReadLine().ToUpper() != "N") return;
                        }
                        break;
                    case "R":
                        break;
                    default:
                        if (step < maxStep)
                        {
                            step++;
                        }
                        else
                        {
                            Console.Write("Are you sure you want to exit this menu? (Y/n):");
                            if (Console.ReadLine().ToUpper() != "N") return;
                        }
                        break;
                }
            }
        } */

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
        /// 
        /// </summary>
        static DateTime RequestReservationStartDate(DateTime? defaultt)
        {
            return DateTime.Today;
        }
    }
}
