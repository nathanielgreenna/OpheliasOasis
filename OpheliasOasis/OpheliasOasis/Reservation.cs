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
using System.Text;
using System.Runtime.Serialization;

namespace OpheliasOasis
{
    [DataContract(Name = "Res", Namespace = "OpheliasOasis")]
    public class Reservation
    {
        [DataMember(Name = "resType")]
        private ReservationType reservationType;
        [DataMember(Name = "resName")]
        private String customerName;
        [DataMember(Name = "resCC")]
        private String customerCreditCard;
        [DataMember(Name = "resEmail")]
        private String customerEmail;
        [DataMember(Name = "roomNo")]
        private int roomNumber;
        [DataMember(Name = "startDate")]
        private DateTime startDate;
        [DataMember(Name = "paymentDate")]
        private DateTime paymentDate;
        [DataMember(Name = "endDate")]
        private DateTime endDate;
        [DataMember(Name = "resPrice")]
        private double totalPrice;
        [DataMember(Name = "resPriceFirstDay")]
        private double firstDayPrice;
        [DataMember(Name = "resStatus")]
        private ReservationStatus reservationStatus;


        public Reservation()
        {
        }
        public Reservation(ReservationType resType, String cusName, String cusCC, DateTime sDate, DateTime eDate) 
        {
            reservationType = resType;
            customerName = cusName;
            customerCreditCard = cusCC;
            startDate = sDate;
            endDate = eDate;
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
            return clone;
        }

        public ReservationType getReservationType() 
        {
            return reservationType;
        }
        public void setReservationType(ReservationType type) 
        {
            reservationType = type;
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
        }
        public String getCustomerEmail() 
        {
            return customerEmail;
        }
        public void setCustomerEmail(String email) 
        {
            customerEmail = email;
        }
        public int getRoomNumber() 
        {
            return roomNumber;
        }
        public void setRoomNumber(int roomNo)
        {
            roomNumber = roomNo;
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
        }

        public double getTotalPrice() 
        {
            return totalPrice;
        }

        public void setTotalPrice(double price)
        {
            totalPrice = price;
        }

        public double getFirstDayPrice()
        {
            return firstDayPrice;
        }

        public void setFirstDayPrice(double price)
        {
            firstDayPrice = price;
        }

        public ReservationStatus getReservationStatus()
        {
            return reservationStatus;
        }
        public void setReservationStatus(ReservationStatus status)
        {
            reservationStatus = status;
        }

        public void cancelReservation() 
        {
            reservationStatus = ReservationStatus.Cancelled;
        }

        public void checkIn()
        {
            reservationStatus = ReservationStatus.CheckedIn;
        }

        public void checkOut()
        {
            reservationStatus = ReservationStatus.CheckedOut;
        }

        public void changeReservation(DateTime sDate, DateTime eDate)
        {
            startDate = sDate;
            endDate = eDate;
        }
    }
}
