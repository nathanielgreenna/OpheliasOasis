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
            Tuple.Create<Func<String, String>, String>(checkPass, "Input manager password (press <Enter> to skip)");
       
        private readonly static Tuple<Func<String, String>, String> emailConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "Send emails and cancel invalid reservations when complete (press <enter> to continue)");

        private readonly static Tuple<Func<String, String>, String> backupConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "Create a backup (press <enter> to continue)");

        private readonly static Tuple<Func<String, String>, String> noShowConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "Charge no-show penalties and cancel reservations when complete (press <enter> to continue)");

        private readonly static Tuple<Func<String, String>, String> assignRoomsConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "Assign rooms for the reservations starting today (press <enter> to continue)");

        private readonly static Tuple<Func<String, String>, String> checkP =
            Tuple.Create<Func<String, String>, String>(checkPasschange, "Input the current manager password");
        private readonly static Tuple<Func<String, String>, String> pass1 =
            Tuple.Create<Func<String, String>, String>(getNewPass, "Input the new manager password (5 or more characters)");
        private readonly static Tuple<Func<String, String>, String> pass2 =
            Tuple.Create<Func<String, String>, String>(confirmNewPass, "Confirm password");





        public static void Init(ReservationDB db, Calendar cl, Hotel hotel,String mp)
        {
            // Initialize references
            rdb = db;
            cal = cl;
            ht = hotel;
            manPass = mp;
            rGen = new ReportGenerator();




            // Initialize menu options

            generateReports = new ProcessPage("Generate Reports", "Generate the manager and employee reports",
                new List<Tuple<Func<String, String>, String>> {
                    genDAR, genDOR, genEOR, genERIR, genIR, checkPassw
                }, GenerateSelectedReports, null);

            generateEmails = new ProcessPage("Update 60-Days Reservations", "Send emails to guests who need to privide their information, and cancel reservations for those who have not provided it in time", new List<Tuple<Func<String, String>, String>> { emailConfirmation }, GenEmails, null);

            createBackups = new ProcessPage("Create Backups", "Create a new backup of the data in the application", new List<Tuple<Func<String, String>, String>> { backupConfirmation }, makeBackup, null);

            chargeNoShow = new ProcessPage("Charge No-Shows", "Cancel reservations for guests who did not arrive and charge them for the first day", new List<Tuple<Func<String, String>, String>> { noShowConfirmation }, ChargeNoShows, null);

            roomAssign = new ProcessPage("Assign Rooms", "Assign rooms to reservations starting today", new List<Tuple<Func<String, String>, String>> { assignRoomsConfirmation }, assignRooms, null);

            changeMPass = new ProcessPage("Change Manager Password", "Change the password for the manager",
                new List<Tuple<Func<String, String>, String>> {
                    checkP, pass1, pass2
                }, changePass, null);

            // Initialize menu
            recordsMenu = new MenuPage("Records", "Generate reports, emails, and backups", new List<Page> { roomAssign, generateReports, generateEmails, createBackups, chargeNoShow, changeMPass });
        }
            






        //----------------REPORTS---------------------

        static String gDAR(String input)
        {
            GenerateReportArray = new List<bool> { false, false, false, false, false};
        
            if(input.ToUpper() != "N") { GenerateReportArray[0] = true;
            }
            else { GenerateReportArray[0] = false; }
            return "";
        }
        static String gDOR(String input)
        {

            if (input.ToUpper() != "N") { GenerateReportArray[1] = true; 
            }
            else { GenerateReportArray[1] = false; }
            return "";
        }
        static String gEOR(String input)
        {

            if (input.ToUpper() != "N") { GenerateReportArray[2] = true; 
            }
            else { GenerateReportArray[2] = false; }
            return "";
        }
        static String gERIR(String input)
        {

            if (input.ToUpper() != "N") { GenerateReportArray[3] = true; 
            }
            else { GenerateReportArray[3] = false; }
            return "";
        }
        static String gIR(String input)
        {

            if (input.ToUpper() != "N") { GenerateReportArray[4] = true; 
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
                    return "Password incorrect";
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
            Console.WriteLine("Cancelled " + c + " reservations and emailed " + e + " customers");
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
                        CreditCardStub.WriteTransaction(res.getCustomerCreditCard(), res.getCustomerName(), "1234 1234 1234 1234", "Ophelia's Oasis", res.GetFirstDayPrice());
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
                    return "Password incorrect";
                }
            return "";
        }


        static String getNewPass(String input) 
        { 
            if(input.Length < 5) 
            {
                return ("Password must be 5 or more characters");
            }
            candidatePass = input;
            return ("");
        }

        static String confirmNewPass(String input)
        {
            if (input != candidatePass)
            {
                return ("Passwords do not match");
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
            Console.Write("Rooms Assigned.");
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
