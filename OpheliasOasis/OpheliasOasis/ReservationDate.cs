/*
 * ReservationDate
 * TODO: Should more or less be done. subject to changes
 * Changelog:
 * 4/4/2022: created/initially coded by Nathaniel
 * 4/20/2022: added public to methods - Nathaniel
 * 4/22/2022: Added isFull method - Alex
*/




using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OpheliasOasis
{

    [DataContract(Name = "ResDate", Namespace = "OpheliasOasis")]
    public class ReservationDate
    {
        [DataMember(Name = "Date")]
        private DateTime date;
        [DataMember(Name = "ResPrice")]
        private int basePrice;
        [DataMember(Name = "Occ")]
        private int occupancy = 0;


        public ReservationDate()
        {
        }

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

        public Boolean IsFull()
        {
            return occupancy == 45;
        }
    }
}
