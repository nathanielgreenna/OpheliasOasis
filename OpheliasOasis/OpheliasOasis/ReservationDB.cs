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


        public List<Reservation> getActiveReservations(DateTime date)
        {
            List<Reservation> reservations = new List<Reservation>();
            Dictionary<DateTime, List<Reservation>>.ValueCollection resColl = reservationByDate.Values;
            foreach (List<Reservation> resCollVal in resColl)
            {
                foreach (Reservation reservation in resCollVal)
                {
                    if (date.CompareTo(reservation.getStartDate()) > 0 || date.CompareTo(reservation.getEndDate()) <= 0)
                    {
                        reservations.Add(reservation);
                    }
                }
            }
            return reservations;
        }

        public void addReservation(Reservation res) 
        {
            try
            {
                reservationByDate[res.getStartDate()].Add(res);
            }
            catch (KeyNotFoundException)
            {
                reservationByDate.Add(res.getStartDate(), new List<Reservation>());
                reservationByDate[res.getStartDate()].Add(res);
            }

            try
            {
                reservationByName[res.getCustomerName()].Add(res);
            }
            catch (KeyNotFoundException)
            {
                reservationByName.Add(res.getCustomerName(), new List<Reservation>());
                reservationByName[res.getCustomerName()].Add(res);
            }





        }





    }
}
