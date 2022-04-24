using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    public static class RecordsPageHandler
    {
        private static Calendar cal;
        private static ReservationDB rdb;
        private static List<Boolean> GenerateReportArray;
        private static String manPass;
        private static ReportGenerator rGen;
        
        // Reservation submenu
        public static MenuPage recordsMenu;

        // Menu options
        public static ProcessPage generateReports;
        public static ProcessPage generateEmails;
        public static ProcessPage createBackups;
        public static void Init(ReservationDB db, Calendar cl, String mp)
        {
            // Initialize references
            rdb = db;
            cal = cl;
            manPass = mp;
            rGen = new ReportGenerator();

            // Initialize menu options
            generateReports = new ProcessPage("Report Generation", "Choose reports to generate. Reports will be generated upon completing this page.",
                new List<String> {
                "generate Daily Arrivals Report? Y/N",
                "generate Daily Occupancy Report? Y/N",
                "generate Expected Occupancy Report? Y/N, Managers Only",
                "generate Expected Income Report? Y/N, Managers Only",
                "generate Incentive Report? Y/N, Managers Only",
                "Input Manager Password (<Enter> to skip)"    },
                
                new List<Func<String, String>> {
                gDAR,
                gDOR,
                gEOR,
                gERIR,
                gIR,
                checkPass}, 
                GenerateSelectedReports);
            generateEmails = new ProcessPage("Update Reservation", "Update an existing reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);
            createBackups = new ProcessPage("Cancel Reservation", "Cancel an existing reservation", new List<String> { "Test" }, new List<Func<String, String>> { Test }, Test);

            // Initialize menu
            recordsMenu = new MenuPage("Records", "Records submenu (Generate Reports, Generate Emails, Create Backups)", new List<Page> { generateReports, generateEmails, createBackups });
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













        //TESTS
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
