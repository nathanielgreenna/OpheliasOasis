/*
 * ReportGenerator
 * TODO: Commenting and testing methods
 * Changelog:
 * 4/20/2022: created/initially coded by Alec
 * 4/21/2022: methods finished by Alec
 * 4/29/2022: Fixed occupancy report off by one date, added asterisks for guests checking out today, changed to not display cancelled reservations - Alex
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    class ReportGenerator
    {

        private string path = "C:\\OpheliasOasis\\Reports\\";

        public void generateDailyArrivalsReport(ReservationDB reservationDB)
        {
            DateTime today = DateTime.Today;
            List<Reservation> reservations = reservationDB.getReservation(today);
            List<String> output = new List<String>();
            int activeReservations = 0;

            reservations.Sort((a, b) => a.getCustomerName().CompareTo(b.getCustomerName()));

            foreach (Reservation reservation in reservations)
            {
                if (reservation.getReservationStatus() != ReservationStatus.Cancelled)
                {
                    output.Add($"Guest: {reservation.getCustomerName()}, Reservation type: {reservation.getReservationType()}, Room: {reservation.getRoomNumber()}, " +
                        $"Departure Date: {reservation.getEndDate().ToShortDateString()}");
                    activeReservations++;
                }
            }

            if (activeReservations == 0)
            {
                output.Add("No arrivals today.");
            }

            string file = path + today.ToString("m") + " Daily Arrivals Report.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    sw.WriteLine(output[i]);
                }
            }
        }

        public void generateDailyOccupancyReport(ReservationDB reservationDB)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;
            List<Reservation> reservations = reservationDB.getActiveReservations(yesterday);
            List<String> output = new List<String>();
            int activeReservations = 0;

            foreach (Reservation reservation in reservations)
            {
                if (reservation.getReservationStatus() != ReservationStatus.Cancelled)
                {
                    output.Add($"Guest: {(reservation.getEndDate().Equals(DateTime.Today) ? "*" : "")}{reservation.getCustomerName()}, " + 
                        $"Reservation type: {reservation.getReservationType()}, Room: {reservation.getRoomNumber()}, Departure date: " + reservation.getEndDate().ToShortDateString());
                    activeReservations++;
                }
            }

            if (activeReservations == 0)
            {
                output.Add("No arrivals today.");
            }

            string file = path + today.ToString("m") + " Daily Occupancy Report.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    sw.WriteLine(output[i]);
                }
            }
        }

        public void generateExpectedOccupancyReport(ReservationDB reservationDB)
        {
            List<String> output = new List<String>();
            int totalOccupancy = 0;
            DateTime date = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(1);
                List<Reservation> reservations = reservationDB.getActiveReservations(date);
                int prepaid = 0;
                int sixtyDay = 0;
                int conventianal = 0;
                int incentive = 0;
                int occupancy = 0;
                foreach (Reservation reservation in reservations)
                {
                    occupancy++;
                    switch (reservation.getReservationType())
                    {
                        case ReservationType.Prepaid:
                            prepaid++;
                            break;
                        case ReservationType.SixtyDay:
                            sixtyDay++;
                            break;
                        case ReservationType.Conventional:
                            conventianal++;
                            break;
                        case ReservationType.Incentive:
                            incentive++;
                            break;
                    }
                }
                output.Add(date.ToShortDateString() + ": Prepaid: " + prepaid + ", 60-Day: " + sixtyDay + ", Conventional: " + conventianal +
                    ", Incentive: " + incentive + ", Total Occupancy: " + occupancy);
                totalOccupancy += occupancy;
            }
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(30);
            double occupancyRate = totalOccupancy / 30;
            output.Add("Average Expected Occupancy Rate from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": " + occupancyRate.ToString("F1"));

            string file = path + DateTime.Today.ToString("m") + " Expected Occupancy Report.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    sw.WriteLine(output[i]);
                }
            }
        }

        public void generateExpectedRoomIncomeReport(ReservationDB reservationDB)
        {
            List<String> output = new List<String>();
            double totalIncome = 0;
            DateTime date = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(1);
                List<Reservation> reservations = reservationDB.getActiveReservations(date);
                double income = 0;
                foreach (Reservation reservation in reservations)
                {
                    income += reservation.GetDatePrice(date);
                }
                totalIncome += income;
                output.Add(date.ToShortDateString() + ": $" + income.ToString("F2"));
            }
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(30);
            double averageIncome = totalIncome / 30;
            output.Add("Total Income from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + totalIncome.ToString("F2"));
            output.Add("Average Income from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + averageIncome.ToString("F2"));

            string file = path + DateTime.Today.ToString("m") + " Expected Income Report.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    sw.WriteLine(output[i]);
                }
            }
        }

        public void generateIncentiveReport(ReservationDB reservationDB, Calendar calendar)
        {
            List<String> output = new List<String>();
            double totalDiscount = 0;
            DateTime date = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(1);
                List<Reservation> reservations = reservationDB.getActiveReservations(date);
                double baseRate = 0;
                double discount = 0;
                foreach (Reservation reservation in reservations)
                {
                    if (reservation.getReservationType().Equals(ReservationType.Incentive))
                    {
                        baseRate = calendar.retrieveDate(date).getBasePrice();
                        discount += (baseRate - reservation.GetDatePrice(date));
                    }
                }
                totalDiscount += discount;
                output.Add(date.ToShortDateString() + ": $" + discount.ToString("F2"));
            }
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(30);
            double averageDicount = totalDiscount / 30;
            output.Add("Total Discount from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + totalDiscount.ToString("F2"));
            output.Add("Average Discount from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + averageDicount.ToString("F2"));

            string file = path + DateTime.Today.ToString("m") + " Incentive Report.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                for (int i = 0; i < output.Count; i++)
                {
                    sw.WriteLine(output[i]);
                }
            }
        }
    }
}
