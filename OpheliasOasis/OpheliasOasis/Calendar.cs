/*
 * Calendar
 * 
 * Stores ReservationDates
 * 
 * TODO: 
 * Changelog:
 * 4/20/2022: created/initially coded by Nathan
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OpheliasOasis
{
    [DataContract(Name = "Cal", Namespace = "OpheliasOasis")]
    public class Calendar
    {
        [DataMember(Name = "calDates")]
        private Dictionary<DateTime, ReservationDate> dates = new Dictionary<DateTime, ReservationDate>();

        
        public Calendar() 
        { 
        }
        /// <summary>
        /// Retrive a date from the calendar
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Increments dates over a span, not including the endDate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void incrementOverSpan(DateTime start, DateTime end) 
        { 
            if(start > end) { throw new ArgumentException("Start is after beginning"); }
            DateTime day = start;

            while(day != end) 
            {
                retrieveDate(day).increaseOccupancy();
                day = day.AddDays(1);
            }

        }
        /// <summary>
        /// Decrements dates over a span, not including the endDate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void decrementOverSpan(DateTime start, DateTime end)
        {
            if (start > end) { throw new ArgumentException("Start is after beginning"); }
            DateTime day = start;

            while (day != end)
            {
                retrieveDate(day).decreaseOccupancy();
                day = day.AddDays(1);
            }

        }
        /// <summary>
        /// Special method for XMLReader, doesn't refresh the date in the Data folder
        /// </summary>
        /// <param name="d"></param>
        public void XMLReaderOnlyAdd(ReservationDate d) 
        {
            dates.Add(d.getDate(),d);
        }
        /// <summary>
        /// Writes the whole calendar to XML in the Data folder
        /// </summary>
        public void WriteCaltoXML() 
        {
            Dictionary<DateTime,ReservationDate>.ValueCollection outDates = dates.Values;

            foreach(ReservationDate d in outDates) 
            {
                XMLreader.changeReservationDate(d);
            }
        }



    }
}
