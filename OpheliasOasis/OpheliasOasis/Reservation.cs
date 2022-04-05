/*
 * Reservation
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/getters and setters coded by Nathaniel
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

        String getReservationType() 
        {
            return reservationType;
        }
        void setReservationType(String type) 
        {
            reservationType = type;
        }
        String getCustomerName() 
        {
            return customerName;
        }
        void setCustomerName(String name) 
        {
            customerName = name;
        }
        int getCustomerCreditCard()
        {
            return customerCreditCard;
        }
        void setCustomerCreditCard(int card)
        {
            customerCreditCard = card;
        }
        String getCustomerEmail() 
        {
            return customerEmail;
        }
        void setCustomerEmail(String email) 
        {
            customerEmail = email;
        }
        int getRoomNumber() 
        {
            return roomNumber;
        }

        //don't know if we should be able to set rooms

        ReservationDate getStartDate() 
        {
            return startDate;
        }

        //don't know if we should be able to directly set startDate

        ReservationDate getEndDate()
        {
            return endDate;
        }

        //don't know if we should be able to directly set endDate

        int getTotalPrice() 
        {
            return totalPrice;
        }

        //don't know if we should be able to directly set price

        String getReservationStatus()
        {
            return reservationStatus;
        }
        void setReservationStatus(String status)
        {
            reservationStatus = status;
        }







    }
}
