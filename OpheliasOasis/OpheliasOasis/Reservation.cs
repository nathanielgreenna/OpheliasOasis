/*
 * Reservation
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/getters and setters coded by Nathaniel
 * 4/19/2022: check in/ check out unnecessary with new changes - Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
 * 4/20/2022: Changed reservationType to type ReservationType - Alex
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
        private int customerCreditCard;
        [DataMember(Name = "resEmail")]
        private String customerEmail;
        [DataMember(Name = "roomNo")]
        private int roomNumber;
        [DataMember(Name = "startDate")]
        private DateTime startDate;
        [DataMember(Name = "endDate")]
        private DateTime paymentDate;
        [DataMember(Name = "paymentDate")]
        private DateTime endDate;
        [DataMember(Name = "resPrice")]
        private int totalPrice;
        [DataMember(Name = "resStatus")]
        private String reservationStatus;


        public Reservation()
        {
        }
        public Reservation(ReservationType resType, String cusName, int cusCC, DateTime sDate, DateTime eDate) 
        {
            reservationType = resType;
            customerName = cusName;
            customerCreditCard = cusCC;
            startDate = sDate;
            endDate = eDate;
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
        public void setCustomerName(String name) 
        {
            customerName = name;
        }
        public int getCustomerCreditCard()
        {
            return customerCreditCard;
        }
        public void setCustomerCreditCard(int card)
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

        public void setStartDate(DateTime t)
        {
            startDate = t;
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

        public int getTotalPrice() 
        {
            return totalPrice;
        }

        public void setTotalPrice(int price)
        {
            totalPrice = price;
        }

        public String getReservationStatus()
        {
            return reservationStatus;
        }
        public void setReservationStatus(String status)
        {
            reservationStatus = status;
        }



        public void cancelReservation() 
        {
            reservationStatus = "Cancelled";
        }

        public void checkIn()
        {
            reservationStatus = "Checked In";
        }

        public void checkOut()
        {
            reservationStatus = "Checked Out";
        }

        public void changeReservation(DateTime sDate, DateTime eDate)
        {
            startDate = sDate;
            endDate = eDate;
        }





    }
}
