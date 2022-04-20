/*
 * ReservationDate
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/initially coded by Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
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

        public ReservationDate(DateTime newDate) 
        {
            date = newDate;
        }
        public DateTime getDate() 
        {
            return (date);
        }


        public int getBasePrice() 
        {
            return (basePrice);
        }
        public void setBasePrice(int newPrice)
        {
            basePrice = newPrice;
        }
        public void increaseOccupancy() 
        {
            occupancy++;
        }
        public void decreaseOccupancy()
        {
            occupancy--;
        }
        public int getOccupancy()
        {
            return (occupancy);
        }


    }
}
