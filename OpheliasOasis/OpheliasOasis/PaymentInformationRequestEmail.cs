/*
 * PaymentInformationRequestEmail
 * 
 * Description: An extension of IEmail designed to generate and store the information for a payment request email.
 * 
 * To do: Change 60-day detection to recognize the enumeration
 * 
 * Changelog:
 * 4/20/2022: Implemented getters for recipients, header text, body text, and attachments & added error checking - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
    /// <summary>
    /// Create and store the information associated with an email requesting payment information for 60-day reservations.
    /// </summary>
    class PaymentInformationRequestEmail : IEmail
    {
        private readonly Reservation reservation;

        /// <summary>
        /// Create an object to generate and store the information for a payment request email.
        /// </summary>
        /// <param name="reservation">The 60-day reservation for the request.</param>
        public PaymentInformationRequestEmail(Reservation reservation)
        {

            String emailName = reservation == null ? "" : reservation.getCustomerName() + "'s PaymentInformationRequestEmail";

            if (reservation == null)
            {
                throw new ArgumentNullException("reservation");
            }
            else if (reservation.getReservationType() != ReservationType.SixtyDay)
            {
                throw new ArgumentException(emailName + ": Reservation type must be 60-day, not " + reservation.getReservationType(), "reservation");
            }
            else if (reservation.getCustomerCreditCard() != 0)
            {
                throw new ArgumentException(emailName + ": Credit card information has already been supplied", "reservation");
            }
            else
            {
                this.reservation = reservation;
            }
        }

		List<string> IEmail.GetRecipients()
		{
			return new List<String>
			{
				reservation.getCustomerEmail()
			};
		}

		string IEmail.GetHeaderText()
		{
			return "Ophelia’s Oasis - Missing Credit Card Information for Your Reservation";
		}

		string IEmail.GetBodyText()
		{
			return "Dear " + reservation.getCustomerName() + ",\n" +
				"Our records indicate that you placed a 60-days-advance reservation but have not provided your payment " +
				"information. You must provide this information by " + reservation.getStartDate().AddDays(-45).ToString("M/d/yyyy") + ", 45 days before your " +
				"reservation starts. Please call us, and an employee will facilitate this process.\n" +
				"Thank you for choosing Ophelia’s Oasis,\n" +
				"The Ophelia’s Oasis staff team";
		}

		List<string> IEmail.GetAttachments()
		{
			return new List<String>();
		}
	}
}