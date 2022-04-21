/*
 * ReportGenerator
 * TODO: Commenting and testing methods
 * Changelog:
 * 4/20/2022: created/initially coded by Alec
 * 4/21/2022: methods finished by Alec
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    class ReportGenerator
    {

        private string path = @"C:\Reports\";

        public void generateDailyArrivalsReport(ReservationDB reservationDB)
        {
            DateTime today = DateTime.Today;
            List<Reservation> reservations = reservationDB.getReservation(today);
            List<String> output = new List<String>();
            if (reservations.Count == 0)
            {
                output.Add("No arrivals today.");
            }
            else
            {
                reservations.Sort((a, b) => a.getCustomerName().CompareTo(b.getCustomerName()));
                foreach (Reservation reservation in reservations)
                {
                    output.Add(reservation.getCustomerName() + ", " + reservation.getReservationType() +
                        ", " + reservation.getRoomNumber() + ", " + reservation.getEndDate().ToShortDateString());
                }
            }

            string file = path + today.ToShortDateString() + @"\DailyArrivalsReport.txt";
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
            DateTime today = DateTime.Today;
            List<Reservation> reservations = reservationDB.getActiveReservations(today);
            List<String> output = new List<String>();
            if (reservations.Count == 0)
            {
                output.Add("No occupants today.");
            }
            else
            {
                reservations.Sort((a, b) => a.getRoomNumber().CompareTo(b.getRoomNumber()));
                foreach (Reservation reservation in reservations)
                {
                    output.Add(reservation.getRoomNumber() + ", " + reservation.getCustomerName() +
                        ", " + reservation.getEndDate().ToShortDateString());
                }
            }

            string file = path + today.ToShortDateString() + @"\DailyOccupancyReport.txt";
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
                date.AddDays(1);
            }
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(30);
            double occupancyRate = totalOccupancy / 30;
            output.Add("Average Expected Occupancy Rate from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": " + occupancyRate.ToString("F1"));

            string file = path + startDate.ToShortDateString() + @"\ExpectedOccupancyReport.txt";
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
            int totalIncome = 0;
            DateTime date = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                List<Reservation> reservations = reservationDB.getActiveReservations(date);
                int income = 0;
                foreach (Reservation reservation in reservations)
                {
                    income += reservation.getTotalPrice();
                }
                totalIncome += income;
                output.Add(date.ToShortDateString() + ": $" + income);
                date.AddDays(1);
            }
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(30);
            double averageIncome = totalIncome / 30;
            output.Add("Total Income from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + totalIncome);
            output.Add("Average Income from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + averageIncome.ToString("F2"));

            string file = path + startDate.ToShortDateString() + @"\ExpectedIncomeReport.txt";
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
            int totalDiscount = 0;
            DateTime date = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                List<Reservation> reservations = reservationDB.getActiveReservations(date);
                int baseRate = calendar.retrieveDate(date).getBasePrice();
                int discount = 0;
                foreach (Reservation reservation in reservations)
                {
                    if (reservation.getReservationType().Equals(ReservationType.Incentive))
                    {
                        discount += (baseRate - reservation.getTotalPrice());
                    }
                }
                totalDiscount += discount;
                output.Add(date.ToShortDateString() + ": $" + discount);
                date.AddDays(1);
            }
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(30);
            double averageDicount = totalDiscount / 30;
            output.Add("Total Discount from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + totalDiscount);
            output.Add("Average Discount from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() + ": $" + averageDicount.ToString("F2"));

            string file = path + startDate.ToShortDateString() + @"\IncentiveReport.txt";
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
