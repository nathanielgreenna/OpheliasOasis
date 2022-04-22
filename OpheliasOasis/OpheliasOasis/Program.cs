/*
 * Program
 * 
 * Description: The controlling class for the program. Loads the database and allows
 * the user to manipulate it. See methodss for more details.
 * 
 * Changelog:
 */

using System;

namespace OpheliasOasis
{
    /// <summary>
    /// The controlling class for Ophelia's Oasis Reservation System
    /// </summary>
    class Program
    {

        private static ReservationDB reservationDB;

        static void Main(string[] args)
        {
            ReservationDB rdb = new ReservationDB();
            Reservation r = new Reservation(ReservationType.SixtyDay, "Bjorkan Ulleholm", 55555555, DateTime.Today, DateTime.Today);
            rdb.addReservation(r);
            Hotel g = new Hotel();
            g.assignRoom();
            Calendar l = new Calendar();
            l.retrieveDate(DateTime.Today);
            XMLreader.XMLout(rdb, g, l);

            ReservationDB rdb2 = new ReservationDB();


            XMLformat totalXML = XMLreader.ResDBin(DateTime.Today);
            rdb2 = totalXML.R;

            r = rdb2.getReservation("Bjorkan Ulleholm")[0];
            r.setCustomerCreditCard(99);
            Console.WriteLine(rdb2.getReservation(DateTime.Today)[0].getCustomerCreditCard());

            while(true)
            {
                System.Threading.Thread.Sleep(2000);
                ReservationMenu();
            }

        }

        /// <summary>
        /// Display the menu for adding, creating, or removing reservations.
        /// </summary>
        static void ReservationMenu()
        {
            // Display menu options
            DisplayMenuHeader("Reservation");
            Console.WriteLine("Please select one of the following options:");
            Console.WriteLine("\t1: Place a new reservation");
            Console.WriteLine("\t2: Update an existing Reservation");
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

            switch (selection)
            {
                case 1: PlaceReservation(); break;
                case 2: UpdateReservation(); break;
                case 3: CancelReservation(); break;
                case 4: return;
            }
        }

        static void PlaceReservation()
        {
            
        }

        static void UpdateReservation()
        {
            Console.WriteLine("Modify");
        }

        static void CancelReservation()
        {
            Console.WriteLine("Cancel");
        }

        static void DisplayMenuHeader(string menuName)
        {
            Console.Clear();
            Console.WriteLine("====================| " + menuName.ToUpper() + " MENU |====================");
            Console.WriteLine();
        }
    }
}
