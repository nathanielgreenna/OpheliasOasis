/*
 * Reservation
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/getters and setters coded by Nathaniel
 * 4/19/2022: check in/ check out unnecessary with new changes - Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
 * 4/20/2022: Changed reservationType to type ReservationType
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    class Reservation
    {
        private ReservationType reservationType;
        private String customerName;
        private int customerCreditCard;
        private String customerEmail;
        private int roomNumber;
        private DateTime startDate;
        private DateTime endDate;
        private int totalPrice;
        private String reservationStatus;



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

        //don't know if we should be able to set rooms

        public DateTime getStartDate() 
        {
            return startDate;
        }

        //don't know if we should be able to directly set startDate

        public DateTime getEndDate()
        {
            return endDate;
        }

        //don't know if we should be able to directly set endDate

        public int getTotalPrice() 
        {
            return totalPrice;
        }

        //don't know if we should be able to directly set price

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
