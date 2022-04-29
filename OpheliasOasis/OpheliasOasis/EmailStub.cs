using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    //Simulates an email-sending API
    public static class EmailStub
    {
        


        public static void sendEmail(PaymentInformationRequestEmail g) 
        {

            String file = "C:\\OpheliasOasis\\EmailCCStubs\\EmailRecords.txt";
            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.WriteLine("Email sent from Ophelia's Oasis to " + g.GetRecipients()[0] + " at " + DateTime.Now.ToString() + ". Subject line was \"" + g.GetHeaderText() + "\"");
            }
            //output to a text file


        }

    }
}
