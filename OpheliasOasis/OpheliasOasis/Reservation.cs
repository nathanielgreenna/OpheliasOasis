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
 * 4/24/2022: Added clone method - Alex
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
        [DataMember(Name = "FPrice")]
        private double firstDayPrice;
        [DataMember(Name = "Status")]
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
        public void setCustomerName(String name) 
        {
            customerName = name;
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

        public void setStartDate(DateTime t)
        {
            startDate = t;
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

        public double getTotalPrice() 
        {
            return totalPrice;
        }

        public void setTotalPrice(double price)
        {
            totalPrice = price;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
        }

        public double getFirstDayPrice()
        {
            return firstDayPrice;
        }

        public void setFirstDayPrice(double price)
        {
            firstDayPrice = price;
            if (ID != 0) { XMLreader.AddOrChangeReservationinDB(this); }
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





    }
}
