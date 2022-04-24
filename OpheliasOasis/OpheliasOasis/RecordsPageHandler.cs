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




        // Initialize menu options
        // Steps - tuples pairing an input-parsing function with the associated prompt - used in ProcessPages
        private readonly static Tuple<Func<String, String>, String> genDAR =
            Tuple.Create<Func<String, String>, String>(gDAR, "generate Daily Arrivals Report? Y/N");
        private readonly static Tuple<Func<String, String>, String> genDOR =
            Tuple.Create<Func<String, String>, String>(gDOR, "generate Daily Occupancy Report? Y/N");
        private readonly static Tuple<Func<String, String>, String> genEOR =
            Tuple.Create<Func<String, String>, String>(gEOR, "generate Expected Occupancy Report? Y/N, Managers Only");
        private readonly static Tuple<Func<String, String>, String> genERIR =
            Tuple.Create<Func<String, String>, String>(gERIR, "generate Expected Income Report? Y/N, Managers Only");
        private readonly static Tuple<Func<String, String>, String> genIR =
            Tuple.Create<Func<String, String>, String>(gIR, "generate Incentive Report? Y/N, Managers Only");
        private readonly static Tuple<Func<String, String>, String> checkPassw =
            Tuple.Create<Func<String, String>, String>(checkPass, "Input Manager Password (<Enter> to skip)");
       
        private readonly static Tuple<Func<String, String>, String> emailConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will send emails and cancel invalid reservations when complete (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> backupConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will create a backup when complete (B to go back, <Enter to continue>)");

        private readonly static Tuple<Func<String, String>, String> noShowConfirmation =
            Tuple.Create<Func<String, String>, String>(generalCheck, "This process will charge no-show penalties and cancel reservations when complete (B to go back, <Enter to continue>)");




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
                }, GenerateSelectedReports);

            generateEmails = new ProcessPage("Handle 60-day Reservation Updates", "Send 60-day emails, cancel 60-day reservations with no credit cards within 30 days.", new List<Tuple<Func<String, String>, String>> { emailConfirmation }, GenEmails);

            createBackups = new ProcessPage("Create Backups", "Creates a new backup, saving data for the next time the application is opened", new List<Tuple<Func<String, String>, String>> { backupConfirmation }, makeBackup);

            chargeNoShow = new ProcessPage("Charge No-Shows", "Charges No-Show penalties and cancels associated reservations", new List<Tuple<Func<String, String>, String>> { noShowConfirmation }, ChargeNoShows);

            // Initialize menu
            recordsMenu = new MenuPage("Records", "Records submenu (Generate Reports, Generate Emails && Cancel no-CCs, Create Backups, Charge No-Shows)", new List<Page> { generateReports, generateEmails, createBackups, chargeNoShow });
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
                    if(res.getReservationType() == ReservationType.SixtyDay && (String.IsNullOrEmpty(res.getCustomerCreditCard()) || ! (res.getReservationStatus().Equals(ReservationStatus.Paid)))) 
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
                    if (res.getReservationType() == ReservationType.SixtyDay && (String.IsNullOrEmpty(res.getCustomerCreditCard()) || !(res.getReservationStatus().Equals(ReservationStatus.Emailed))))
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
                if (!res.getReservationStatus().Equals(ReservationStatus.CheckedIn))
                {
                    CreditCardStub.chargeNoShowPenalty(res);
                    res.cancelReservation();
                    c++;
                }
            }
            Console.WriteLine("Charged " + c + " no-shows and cancelled their reservations.");
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
