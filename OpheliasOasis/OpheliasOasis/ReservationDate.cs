/*
 * ReservationDate
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/initially coded by Nathaniel
*/




using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    

    class ReservationDate
    {
        private DateTime date;
        private int basePrice;
        private int occupancy = 0;

        ReservationDate(DateTime newDate) 
        {
            date = newDate;
        }
        DateTime getDate() 
        {
            return (date);
        }


        int getBasePrice() 
        {
            return (basePrice);
        }
        void setBasePrice(int newPrice)
        {
            basePrice = newPrice;
        }
        void increaseOccupancy() 
        {
            occupancy++;
        }
        void decreaseOccupancy()
        {
            occupancy--;
        }
        int getOccupancy()
        {
            return (occupancy);
        }


    }
}
