/*
 * ReservationStatus
 * 
 * Description: Defines the possible reservation stauses
 * 
 * Changelog:
 * 4/24/2022: Initial code - Alec
 * 4/24/2022: Added "Placed" - Alex
 */

using System;

namespace OpheliasOasis
{
    /// <summary>
    /// An enum representing the status of the reservation.
    /// </summary>
    public enum ReservationStatus
    {
        /// <summary>
		/// Initial state of a reservation.
		/// </summary>
        Placed,

        /// <summary>
		/// Credit card information pending (SixtyDay Reservations).
		/// </summary>
        Emailed,

        /// <summary>
		/// Credit card information provided.
		/// </summary>
        Paid,

        /// <summary>
		/// Customer has checked in.
		/// </summary>
        CheckedIn,

        /// <summary>
        /// Customer has checked out.
        /// </summary>
        CheckedOut,

        /// <summary>
        /// Reservation has been cancelled.
        /// </summary>
        Cancelled
    }
}
