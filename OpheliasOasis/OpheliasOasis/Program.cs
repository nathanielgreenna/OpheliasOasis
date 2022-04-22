using System;

namespace OpheliasOasis
{


    class Program
    {
        static void Main(string[] args)
        {
            ReservationDB rdb = new ReservationDB();
            Reservation r = new Reservation(ReservationType.SixtyDay, "Bjorkan Ulleholm", 55555555, DateTime.Today, DateTime.Today);
            rdb.addReservation(r);
            Hotel g = new Hotel();
            g.assignRoom();
            Calendar l = new Calendar();
            l.retrieveDate(DateTime.Today);
            XMLreader.XMLout(rdb, g, l);

            ReservationDB rdb2 = new ReservationDB();


            XMLformat totalXML = XMLreader.ResDBin(DateTime.Today);
            rdb2 = totalXML.R;

            r = rdb2.getReservation("Bjorkan Ulleholm")[0];
            r.setCustomerCreditCard(99);
            Console.WriteLine(rdb2.getReservation(DateTime.Today)[0].getCustomerCreditCard());


        }
    }
}
