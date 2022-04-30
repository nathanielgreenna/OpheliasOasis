/*
 * CreditCardStuB
 * 
 * Description: Simulates a credit card API.
 * 
 * Changelog:
 * 4/24/2022: Initial code - Alec
 * 4/24/2022: Added WriteTransaction method - Alex
 * 4/25/2022: Refactored project to use WriteTransaction instead of chargeNoShowPenalty(Reservation) and chargeCreditCard(Reservation) & formatted header comment - Alex
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    /// <summary>
    /// Simulates a credit card API by storing charges into text files.
    /// </summary>
    public static class CreditCardStub
    {
        /// <summary>
        /// "Charge" a credit card. The inputs are credit card numbers, which admittedly would not be used in real life.
        /// </summary>
        /// <param name="srcAccountNumber">A string containing the paying account number.</param>
        /// <param name="srcAccountName">A string containing the name on the paying account.</param>
        /// <param name="destAccountNumber">A string containing the recieving account number.</param>
        /// <param name="destAccountName">A string containing the name on the reciving account.</param>
        /// <param name="amt">A double containing the amount transacted.</param>
        public static void WriteTransaction(String srcAccountNumber, String srcAccountName, String destAccountNumber, String destAccountName, double amt)
        {
            String file = "C:\\OpheliasOasis\\EmailCCStubs\\TransactionRecord.txt";
            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.WriteLine($"[{DateTime.Today.ToShortDateString()}] {amt:C2} transferred from {srcAccountName} ({srcAccountNumber}) to {destAccountName} ({destAccountNumber})");
            }

        }
    }
}
