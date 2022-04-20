/*
 * ReservationDB
 * TODO: testing
 * Changelog:
 * 4/20/2022: created/initially coded by Nathan
*/


using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliasOasis
{
    class ReservationDB
    {
        private Dictionary<String, List<Reservation>> reservationByName = new Dictionary<String, List<Reservation>>();
        private Dictionary<DateTime, List<Reservation>> reservationByDate = new Dictionary<DateTime, List<Reservation>>();

        public List<Reservation> getReservation(DateTime date) 
        {
            try
            {
                return (reservationByDate[date]);
            }
            catch(KeyNotFoundException)
            {
                return (null);
            }
        }

        public List<Reservation> getReservation(String name)
        {
            try
            {
                return (reservationByName[name]);
            }
            catch (KeyNotFoundException)
            {
                return (null);
            }
        }




        public void addReservation(Reservation res) 
        {
            List<Reservation> byNameList;
            List<Reservation> byDateList;
            try
            {
                byNameList = reservationByName[res.getCustomerName()];
                byDateList = reservationByDate[res.getStartDate().getDate()];
                byNameList.Add(res);
                byDateList.Add(res);
            }
            catch (KeyNotFoundException)
            {
                reservationByName.Add(res.getCustomerName(),new List<Reservation>());
                reservationByDate.Add(res.getStartDate().getDate(), new List<Reservation>());
                reservationByName[res.getCustomerName()].Add(res);
                reservationByDate[res.getStartDate().getDate()].Add(res);
            }
        }




    }
}
