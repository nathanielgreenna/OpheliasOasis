/*
 * Reservation
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/getters and setters coded by Nathaniel
 * 4/19/2022: check in/ check out unnecessary with new changes - Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
 * 4/20/2022: Changed reservationType to type ReservationType - Alex
 * 4/23/2022: Changed customerCreditCard to string and price to double - Alex
 * 4/24/2022: Changed reservationStatus to type ReservationStatus - Alec
 * 4/24/2022: Added clone method & replaced setters for customerName and startDate with "with<Field>" - Alex
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpheliasOasis
{
    [DataContract(Name = "Res", Namespace = "OpheliasOasis")]
    public class Reservation
    {
        [DataMember(Name = "ID")]
        private uint ID = 0;
        [DataMember(Name = "Type")]
        private ReservationType reservationType;
        [DataMember(Name = "Name")]
        private String customerName;
        [DataMember(Name = "CC")]
        private String customerCreditCard;
        [DataMember(Name = "Email")]
        private String customerEmail;
        [DataMember(Name = "roomNo")]
        private int roomNumber;
        [DataMember(Name = "sDate")]
        private DateTime startDate;
        [DataMember(Name = "payDate")]
        private DateTime paymentDate;
        [DataMember(Name = "eDate")]
        private DateTime endDate;
        [DataMember(Name = "Price")]
        private double totalPrice;
        [DataMember(Name = "Status")]
        private ReservationStatus reservationStatus;
        [DataMember(Name = "Prices")]
        private List<double> prices = new List<double>();
        [DataMember(Name = "Paid")]
        private bool paid;


        public Reservation()
        {
            paid = false;
        }
        public Reservation(ReservationType resType, String cusName, String cusCC, DateTime sDate, DateTime eDate) 
        {
            paid = false;
            reservationType = resType;
            customerName = cusName;
            customerCreditCard = cusCC;
            startDate = sDate;
            endDate = eDate;
        }

        public uint getID()
        {
            return ID;
        }
        public void setID(uint newID)
        {
            if (ID == 0)
            {
                ID = newID;
            }
        }
        public Reservation Clone()
        {
            Reservation clone = new Reservation();
            clone.reservationType = reservationType;
            clone.customerName = customerName;
            clone.customerCreditCard = customerCreditCard;
            clone.customerEmail = customerEmail;
            clone.roomNumber = roomNumber;
            clone.startDate = startDate;
            clone.paymentDate = paymentDate;
            clone.endDate = endDate;
            clone.totalPrice = totalPrice;
            clone.reservationStatus = reservationStatus;
            clone.prices = new List<double>(prices);
            clone.paid = paid;
            return clone;
        }

        public ReservationType getReservationType() 
        {
            return reservationType;
        }
        public void setReservationType(ReservationType type) 
        {
            reservationType = type;
            if(ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }
        public String getCustomerName() 
        {
            return customerName;
        }
        /// <summary>
        /// Used in lieu of a setter, since customerName is a key for the ReservationDB.
        /// </summary>
        /// <param name="name">The new customer name.</param>
        /// <returns>A copy of the reservation with the new customer name.</returns>
        public Reservation WithCustomerName(String name)
        {
            Reservation withName = this.Clone();
            withName.customerName = name;
            return withName;
        }

        public String getCustomerCreditCard()
        {
            return customerCreditCard;
        }
        public void setCustomerCreditCard(String card)
        {
            customerCreditCard = card;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }
        public String getCustomerEmail() 
        {
            return customerEmail;
        }
        public void setCustomerEmail(String email) 
        {
            customerEmail = email;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }
        public int getRoomNumber() 
        {
            return roomNumber;
        }
        public void setRoomNumber(int roomNo)
        {
            roomNumber = roomNo;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public DateTime getStartDate() 
        {
            return startDate;
        }

        /// <summary>
        /// Used in lieu of a setter, since startDate is a key for the ReservationDB.
        /// </summary>
        /// <param name="t">The new start date.</param>
        /// <returns>A copy of the reservation with the new start date.</returns>
        public Reservation WithStartDate(DateTime t)
        {
            Reservation withDate = this.Clone();
            withDate.startDate = t;
            return withDate;
        }

        public void setPaymentDate(DateTime t)
        {
            paymentDate = t;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public DateTime getPaymentDate()
        {
            return paymentDate;
        }

        public DateTime getEndDate()
        {
            return endDate;
        }

        public void setEndDate(DateTime t)
        {
            endDate = t;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        /// <summary>
        /// Get the total price for all nights of the reservation.
        /// </summary>
        /// <returns>A double containing the total price</returns>
        public double GetTotalPrice() 
        {
            return totalPrice;
        }

        /// <summary>
        /// Get the price of the first day. Used in calculating fees.
        /// </summary>
        /// <returns>A double containing the cost of the first night of the reservation.</returns>
        public double GetFirstDayPrice()
        {
            return prices[0];
        }

        /// <summary>
        /// Return the price for a given day.
        /// </summary>
        /// <param name="date">The date for which to get the price.</param>
        /// <returns>A double containing the price for the date provided.</returns>
        public double GetDatePrice(DateTime date)
        {
            if (date >= getStartDate() && date < getEndDate())
            {
                return prices[(int)(date - getStartDate()).TotalDays];
            }

            return 0;
        }

        /// <summary>
        /// Set the prices for a reservation. Throws an ArgumentException if the wrong number of parameters are provided.
        /// </summary>
        /// <param name="prices">A list of doubles, each representing the price of the ith day.</param>
        public void SetPrices(List<double> prices)
        {
            // Input validation
            int days = (int)(getEndDate() - getStartDate()).TotalDays;
            if (prices.Count != days)
            {
                throw new ArgumentException("The number of prices and number of days for this reservation do not match up.");
            }

            // Variable setting
            this.prices = new List<double>(prices);

            totalPrice = 0;
            for (int i = 0; i < days; i++)
            {
                totalPrice += prices[i];
            }
        }

        public bool IsPaid()
        {
            return paid;
        }

        public void SetPaid(bool paid)
        {
            this.paid = paid;

            setPaymentDate(DateTime.Now);
        }

        public ReservationStatus getReservationStatus()
        {
            return reservationStatus;
        }
        public void setReservationStatus(ReservationStatus status)
        {
            reservationStatus = status;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public void cancelReservation() 
        {
            reservationStatus = ReservationStatus.Cancelled;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public void checkIn()
        {
            reservationStatus = ReservationStatus.CheckedIn;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public void checkOut()
        {
            reservationStatus = ReservationStatus.CheckedOut;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public void changeReservation(DateTime sDate, DateTime eDate)
        {
            startDate = sDate;
            endDate = eDate;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public override string ToString()
        {
            return $"{reservationType} Reservation from {startDate.ToShortDateString()} to {endDate.ToShortDateString()} ({(paid ? "Paid" : "Unpaid")}, {reservationStatus}, " +
                $"Credit card: {(String.IsNullOrEmpty(customerCreditCard) ? "Not provided" : $"XXXX XXXX XXXX {customerCreditCard.Split(" ")[3]}")}, " +
                $"Email: {(String.IsNullOrEmpty(customerEmail) ? "Not provided" : customerEmail)})";
        }
    }
}
