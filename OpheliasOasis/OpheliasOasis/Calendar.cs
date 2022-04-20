﻿/*
 * Hotel
 * 
 * retrieveDate requires that your DateTime has a time of 00:00:00. If it doesn't, this will return null. Otherwise, it will always return a ReservationDate, even if it isn't previously created.
 * 
 * TODO: 
 * Changelog:
 * 4/20/2022: created/initially coded by Nathan
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    class Calendar
    {
        private Dictionary<DateTime, ReservationDate> dates = new Dictionary<DateTime, ReservationDate>();


        public ReservationDate retrieveDate(DateTime date) 
        {
            DateTime today = DateTime.Today;
            if (date == null ||  date.TimeOfDay != today.TimeOfDay) 
            {
                return(null);
            }

            try
            {
                return (dates[date]);
            }
            catch(KeyNotFoundException)
            {
                dates.Add(date, new ReservationDate(date));
                return (dates[date]);
            }





        }





    }
}