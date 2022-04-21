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
            r = rdb.getReservation("Bjorkan Ulleholm")[0];
            r.setCustomerCreditCard(99);
            r = rdb.getReservation(DateTime.Today)[0];

            Console.WriteLine(r.getCustomerCreditCard());

        }
    }
}
