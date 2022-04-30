using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    public static class RecordsPageHandler
    {
        private static Calendar cal;
        private static ReservationDB rdb;
        private static Hotel ht;
        private static List<Boolean> GenerateReportArray;
        private static String manPass;
        private static ReportGenerator rGen;
        
        // Reservation submenu
        public static MenuPage recordsMenu;

        // Menu options
        public static ProcessPage generateReports;
        public static ProcessPage generateEmails;
        public static ProcessPage createBackups;
        public static ProcessPage chargeNoShow;
        public static ProcessPage changeMPass;
        public static ProcessPage roomAssign;




        // Initialize menu options
        // Steps - tuples pairing an input-parsing function with the associated prompt - used in ProcessPages
        private readonly static Tuple<Func<String, String>, String> genDAR =
            Tuple.Create<Func<String, String>, String>(gDAR, "generate Daily Arrivals Report? Y/n");
        private readonly static Tuple<Func<String, String>, String> genDOR =
            Tuple.Create<Func<String, String>, String>(gDOR, "generate Daily Occupancy Report? Y/n");
        private readonly static Tuple<Func<String, String>, String> genEOR =
            Tuple.Create<Func<String, String>, String>(gEOR, "generate Expected Occupancy Report? Y/n, Managers Only");
        private readonly static Tuple<Func<String, String>, String> genERIR =
            Tuple.Create<Func<String, String>, String>(gERIR, "generate Expected Income Report? Y/n, Managers Only");
        private readonly static Tuple<Func<String, String>, String> genIR =
            Tuple.Create<Func<String, String>, String>(gIR, "generate Incentive Report? Y/n, Managers Only");
        private readonly static Tuple<Func<String, String>, String> checkPassw =
            Tuple.Create<Func<String, String>, String>(checkPass, "Input Manager Password (<Enter> to skip)");
       
        private readonly static Tuple<Func<String, String>, String> emailConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will send emails and cancel invalid reservations when complete (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> backupConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will create a backup when complete (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> noShowConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will charge no-show penalties and cancel reservations when complete (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> assignRoomsConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will assign rooms to the day's guests (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> checkP =
            Tuple.Create<Func<String, String>, String>(checkPasschange, "Input Current Manager Password");
        private readonly static Tuple<Func<String, String>, String> pass1 =
            Tuple.Create<Func<String, String>, String>(getNewPass, "Input New Manager Password (5 or more characters)");
        private readonly static Tuple<Func<String, String>, String> pass2 =
            Tuple.Create<Func<String, String>, String>(confirmNewPass, "Confirm Password");





        public static void Init(ReservationDB db, Calendar cl, Hotel hotel,String mp)
        {
            // Initialize references
            rdb = db;
            cal = cl;
            ht = hotel;
            manPass = mp;
            rGen = new ReportGenerator();




            // Initialize menu options

            generateReports = new ProcessPage("Report Generation", "Choose reports to generate. Reports will be generated upon completing this page.",
                new List<Tuple<Func<String, String>, String>> {
                    genDAR, genDOR, genEOR, genERIR, genIR, checkPassw
                }, GenerateSelectedReports, null);

            generateEmails = new ProcessPage("Handle 60-day Reservation Updates", "Send 60-day emails, cancel 60-day reservations with no credit cards within 30 days.", new List<Tuple<Func<String, String>, String>> { emailConfirmation }, GenEmails, null);

            createBackups = new ProcessPage("Create Backups", "Creates a new backup, saving data for the next time the application is opened", new List<Tuple<Func<String, String>, String>> { backupConfirmation }, makeBackup, null);

            chargeNoShow = new ProcessPage("Charge No-Shows", "Charges No-Show penalties and cancels associated reservations", new List<Tuple<Func<String, String>, String>> { noShowConfirmation }, ChargeNoShows, null);

            roomAssign = new ProcessPage("Assign Rooms for Today's Arrivals", "Assigns a room to each guest arriving today", new List<Tuple<Func<String, String>, String>> { assignRoomsConfirmation }, assignRooms, null);

            changeMPass = new ProcessPage("Change Manager Password", "Choose manager password. Password will be changed upon completing this page.",
                new List<Tuple<Func<String, String>, String>> {
                    checkP, pass1, pass2
                }, changePass, null);

            // Initialize menu
            recordsMenu = new MenuPage("Records", "Records submenu (Generate Reports, Generate Emails && Cancel no-CCs, Create Backups, Charge No-Shows)", new List<Page> { roomAssign, generateReports, generateEmails, createBackups, chargeNoShow, changeMPass });
        }
            






        //----------------REPORTS---------------------

        static String gDAR(String input)
        {
            GenerateReportArray = new List<bool> { false, false, false, false, false};
        
            if(input.ToUpper() == "Y") { GenerateReportArray[0] = true;
            }
            else { GenerateReportArray[0] = false; }
            return "";
        }
        static String gDOR(String input)
        {

            if (input.ToUpper() == "Y") { GenerateReportArray[1] = true; 
            }
            else { GenerateReportArray[1] = false; }
            return "";
        }
        static String gEOR(String input)
        {

            if (input.ToUpper() == "Y") { GenerateReportArray[2] = true; 
            }
            else { GenerateReportArray[2] = false; }
            return "";
        }
        static String gERIR(String input)
        {

            if (input.ToUpper() == "Y") { GenerateReportArray[3] = true; 
            }
            else { GenerateReportArray[3] = false; }
            return "";
        }
        static String gIR(String input)
        {

            if (input.ToUpper() == "Y") { GenerateReportArray[4] = true; 
            }
            else { GenerateReportArray[4] = false; }
            return "";
        }

        static String checkPass(String input)
        {
            if(GenerateReportArray[2] || GenerateReportArray[3] || GenerateReportArray[4])
            {
                if(input != manPass)
                {
                    return "Password incorrect.";
                }
            }
            return "";
            
        }

        static String GenerateSelectedReports()
        {
            if (GenerateReportArray[0]) { rGen.generateDailyArrivalsReport(rdb); }
            if (GenerateReportArray[1]) { rGen.generateDailyOccupancyReport(rdb); }
            if (GenerateReportArray[2]) { rGen.generateExpectedOccupancyReport(rdb); }
            if (GenerateReportArray[3]) { rGen.generateExpectedRoomIncomeReport(rdb); }
            if (GenerateReportArray[4]) { rGen.generateIncentiveReport(rdb, cal); }
            return "";
        }


        //--------------------Email&CancelNoCCs------------------------


        static String generalCheck(String input)
        {
            return "";
        }

        static String GenEmails()
        {
            int c = 0;
            int e = 0;
            List<Reservation> dayReservations;
            for(int daysinfuture = 1; daysinfuture <= 30; daysinfuture++)
            {
                dayReservations = rdb.getReservation(DateTime.Today.AddDays(daysinfuture));
                foreach(Reservation res in dayReservations)
                {
                    if(res.getReservationType() == ReservationType.SixtyDay && (String.IsNullOrEmpty(res.getCustomerCreditCard()) && ! (res.getReservationStatus().Equals(ReservationStatus.Confirmed) || res.getReservationStatus().Equals(ReservationStatus.Cancelled)))) 
                    {
                        res.cancelReservation();
                        cal.decrementOverSpan(res.getStartDate(),res.getEndDate());
                        c++;
                    }
                }
            }
            
            for (int daysinfuture = 31; daysinfuture <= 45; daysinfuture++)
            {
                dayReservations = rdb.getReservation(DateTime.Today.AddDays(daysinfuture));
                foreach (Reservation res in dayReservations)
                {
                    if (res.getReservationType() == ReservationType.SixtyDay && (String.IsNullOrEmpty(res.getCustomerCreditCard())) && ! (res.getReservationStatus().Equals(ReservationStatus.Emailed) || res.getReservationStatus().Equals(ReservationStatus.Confirmed) || res.getReservationStatus().Equals(ReservationStatus.Cancelled)))
                    {
                        res.setReservationStatus(ReservationStatus.Emailed);
                        EmailStub.sendEmail(new PaymentInformationRequestEmail(res));
                        e++;
                    }
                }
            }
            Console.WriteLine("cancelled " + c + " reservations and emailed " + e + " customers");
            System.Threading.Thread.Sleep(2000);
            return "";

        }


        //----------------------------Backups------------------------------------
        static String makeBackup() 
        {
            XMLreader.XMLout(rdb, ht, cal, manPass);
            return "";
        }


        //--------------------NoShows------------------------
        static String ChargeNoShows()
        {
            int c = 0;
            List<Reservation> dayReservations;
            dayReservations = rdb.getReservation(DateTime.Today.Subtract(TimeSpan.FromDays(1)));
            foreach (Reservation res in dayReservations)
            {
                if ( !(res.getReservationStatus() == ReservationStatus.CheckedIn || res.getReservationStatus() == ReservationStatus.CheckedOut || res.getReservationStatus() == ReservationStatus.Cancelled))
                {
                    if(res.getReservationType() == ReservationType.Conventional || res.getReservationType() == ReservationType.Incentive)
                    {
                        CreditCardStub.WriteTransaction(res.getCustomerCreditCard(), res.getCustomerName(), "Ophelia's Oasis", "1234 1234 1234 1234", res.GetFirstDayPrice());
                    }
                    ht.clearRoom(res.getRoomNumber());
                    res.cancelReservation();

                    cal.decrementOverSpan(res.getStartDate(), res.getEndDate());




                    c++;
                }
            }
            Console.WriteLine("Charged " + c + " no-shows, un-assigned their rooms, and cancelled their reservations.");
            System.Threading.Thread.Sleep(2000);
            return "";
        }


        //------------------------Change Manager Password---------------------------
        private static String candidatePass;


        static String checkPasschange(String input)
        {
                if (input != manPass)
                {
                    return "Password incorrect.";
                }
            return "";
        }


        static String getNewPass(String input) 
        { 
            if(input.Length < 5) 
            {
                return ("Must be 5 or more characters");
            }
            candidatePass = input;
            return ("");
        }

        static String confirmNewPass(String input)
        {
            if (input != candidatePass)
            {
                return ("Must match Step 2 input");
            }
            return ("");
        }

        static String changePass() 
        {
            Program.setPassword(candidatePass);
            manPass = candidatePass;
            XMLreader.changeMPass(manPass);
            DatesPageHandler.setPassword(manPass);
            Console.WriteLine("Password changed!");
            System.Threading.Thread.Sleep(2000);
            return "";
        }

        //------------------Assign Rooms for the day----------------------  
        
        static String assignRooms()
        {
            foreach(Reservation curr in rdb.getReservation(DateTime.Today)) 
            {
                if(curr.getReservationStatus() != ReservationStatus.Cancelled && curr.getRoomNumber() <= 0)
                curr.setRoomNumber(ht.assignRoom());
            }
            Console.Write("Rooms Assigned");
            System.Threading.Thread.Sleep(2000);
            return "";
        }



        //-----------------TESTS----------------------------------
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
