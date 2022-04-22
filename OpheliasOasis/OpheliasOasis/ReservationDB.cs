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

        public ReservationDB()
        {
        }



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



                }



            }
        
        
        
        
        }





    }
}
