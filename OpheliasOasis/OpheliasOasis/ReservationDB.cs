/*
 * ReservationDB
 * TODO: testing
 * Changelog:
 * 4/20/2022: created/initially coded by Nathan
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OpheliasOasis
{
    [DataContract(Name = "ResDB", Namespace = "OpheliasOasis")]
    public class ReservationDB
    {
        [DataMember(Name = "resByName")]
        private Dictionary<String, List<Reservation>> reservationByName = new Dictionary<String, List<Reservation>>();
        private Dictionary<DateTime, List<Reservation>> reservationByDate = new Dictionary<DateTime, List<Reservation>>();
        [DataMember(Name = "maxID")]
        private uint idcount = 1; 

        public ReservationDB()
        {
        }


        /// <summary>
        /// returns a list of reservations starting on a given day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Reservation> getReservation(DateTime date) 
        {
            try
            {
                return (new List<Reservation>(reservationByDate[date]));
            }
            catch(KeyNotFoundException)
            {
                return (new List<Reservation>());
            }
        }
        /// <summary>
        /// returns reservations for a particular name (two customers could have the same name)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Reservation> getReservation(String name)
        {
            try
            {
                
                return (new List<Reservation>(reservationByName[name]));
            }
            catch (KeyNotFoundException)
            {
                return (new List<Reservation>());
            }
        }

        /// <summary>
        /// returns a list of reservations happening now (or cancelled but were set to happen now).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Reservation> getActiveReservations(DateTime date)
        {
            List<Reservation> reservations = new List<Reservation>();
            Dictionary<DateTime, List<Reservation>>.ValueCollection resColl = reservationByDate.Values;
            foreach (List<Reservation> resCollVal in resColl)
            {
                foreach (Reservation reservation in resCollVal)
                {
                    if (date.CompareTo(reservation.getStartDate()) >= 0 && date.CompareTo(reservation.getEndDate()) < 0)
                    {
                        reservations.Add(reservation);
                    }
                }
            }
            return reservations;
        }
        /// <summary>
        /// adds a reservation to the database
        /// </summary>
        /// <param name="res"></param>
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

            res.setID(idcount);
            XMLreader.AddOrChangeReservationinDB(res);
            idcount++;
        }
        /// <summary>
        /// special XMLReader function, doesn't assign new IDs when adding to the DB
        /// </summary>
        /// <param name="res"></param>
        public void addReservationReader(Reservation res)
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

            if(res.getID() >= idcount) 
            {
                idcount = res.getID() + 1;
            }

        }
        /// <summary>
        /// replaces one reservation with another, giving the new one the old one's ID
        /// </summary>
        /// <param name="oldRes"></param>
        /// <param name="newRes"></param>
        public void replaceReservation(Reservation oldRes, Reservation newRes) 
        {
            if (newRes == null) { throw new ArgumentException("Second Argument Null"); }
            List<Reservation> nameList;
            List<Reservation> dateList;
            
            try
            {
                nameList = reservationByName[oldRes.getCustomerName()];
                dateList = reservationByDate[oldRes.getStartDate()];
                nameList.Remove(oldRes);
                dateList.Remove(oldRes);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Source reservation not found in database");
            }
            newRes.setID(oldRes.getID());
            addReservationReader(newRes);
            XMLreader.AddOrChangeReservationinDB(newRes);
        }


        /// <summary>
        /// When restoring from a backup, reads the ReservationByDate dictionary into the ReservationByName dictionary
        /// </summary>
        public void reorganize()
        {
            if (reservationByDate != null) 
            {
                return;
            }

            reservationByDate = new Dictionary<DateTime, List<Reservation>>();
            Dictionary<String, List<Reservation>>.ValueCollection lists = reservationByName.Values;


            foreach (List<Reservation> transferResList in lists)
            {
                foreach(Reservation transferRes in transferResList)
                {
                    try
                    {
                        reservationByDate[transferRes.getStartDate()].Add(transferRes);
                    }
                    catch (KeyNotFoundException)
                    {
                        reservationByDate.Add(transferRes.getStartDate(), new List<Reservation>());
                        reservationByDate[transferRes.getStartDate()].Add(transferRes);
                    }
                    XMLreader.AddOrChangeReservationinDB(transferRes);
                }



            }
        
        
        
        
        }





    }
}
