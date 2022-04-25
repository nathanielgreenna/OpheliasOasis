/*
 * CreditCardStuB
 * 
 * Description: Simulates a credit card API.
 * 
 * Changelog:
 * 4/24/2022: Initial code - Alec
 * 4/24/2022: Added WriteTransaction method
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    // summary
    // Simulates a credit card API by storing charges into text files.
    // smmary
    public static class CreditCardStub
    {
        private static string path = "C:\\OpheliasOasis\\Charges\\";
        private static readonly StreamWriter sw = new StreamWriter("C:\\OpheliasOasis\\Charges\\TransactionRecord.txt");

        // summary
        // Charges a customer's credit card for their reservation.
        // summary
        /// <param name="res">The reservation to be charged.</param>
        public static void chargeCreditCard(Reservation res)
        {
            DateTime today = DateTime.Today;
            String output = "Payment of $" + res.getTotalPrice() + " charged to " + res.getCustomerName() + " on " +
                today.ToShortDateString() + " with Credit Card: " + res.getCustomerCreditCard() + ".";
            String file = path + today.ToShortDateString() + @"\" + res.getCustomerName() + "Payment.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                sw.Write(output);
            }
        }

        // summary
        // Charges a customer's credit card a no-show penalty for failing to check-in on time.
        // summary
        /// <param name="res">The reservation to be charged.</param>
        public static void chargeNoShowPenalty(Reservation res)
        {
            DateTime today = DateTime.Today;
            String output = "No-Show penalty of $" + res.getFirstDayPrice() + " charged to " + res.getCustomerName() + " on " + today.ToShortDateString() + ".";
            String file = path + today.ToShortDateString() + @"\" + res.getCustomerName() + "Penalty.txt";
            using (StreamWriter sw = File.CreateText(file))
            {
                sw.Write(output);
            }
        }

        public static void WriteTransaction(String srcAccount, String destAccount, double amt)
        {
            sw.WriteLine($"[{DateTime.Today.ToShortDateString()}] {amt:C2} transferred from {srcAccount} to {destAccount}");
        }
    }
}
