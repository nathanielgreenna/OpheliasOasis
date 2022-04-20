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
