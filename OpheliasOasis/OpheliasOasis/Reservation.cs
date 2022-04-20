/*
 * Reservation
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/getters and setters coded by Nathaniel
 * 4/19/2022: check in/ check out unnecessary with new changes - Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    class Reservation
    {
        private String reservationType;
        private String customerName;
        private int customerCreditCard;
        private String customerEmail;
        private int roomNumber;
        private ReservationDate startDate;
        private ReservationDate endDate;
        private int totalPrice;
        private String reservationStatus;



        public Reservation(String resType, String cusName, int cusCC, ReservationDate sDate, ReservationDate eDate) 
        {
            reservationType = resType;
            customerName = cusName;
            customerCreditCard = cusCC;
            startDate = sDate;
            endDate = eDate;
        }
        public String getReservationType() 
        {
            return reservationType;
        }
        public void setReservationType(String type) 
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

        public ReservationDate getStartDate() 
        {
            return startDate;
        }

        //don't know if we should be able to directly set startDate

        public ReservationDate getEndDate()
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

        public void changeReservation(ReservationDate sDate, ReservationDate eDate)
        {
            startDate = sDate;
            endDate = eDate;
        }





    }
}
