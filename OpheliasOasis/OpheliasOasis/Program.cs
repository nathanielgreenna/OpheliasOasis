using System;

namespace OpheliasOasis
{


    class Program
    {
        static void Main(string[] args)
        {
            Calendar c = new Calendar();
            DateTime t = DateTime.Today;
            ReservationDate r = c.retrieveDate(t);
            r.increaseOccupancy();
            r.increaseOccupancy();
            DateTime t2 = t.AddDays(1);
            r = c.retrieveDate(t2);
            Console.WriteLine(r.getOccupancy());
            r = c.retrieveDate(t);
            Console.WriteLine(r.getOccupancy());

        }
    }
}
