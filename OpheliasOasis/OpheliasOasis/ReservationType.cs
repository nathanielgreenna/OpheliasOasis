/*
 * ReservationType
 * 
 * Description: Defines the possible reservation types
 * 
 * Changelog:
 * 4/20/2022: Initial code - Alex
 * 4/21/2022: Updated documentation - description of reservation types - Alex
 */

using System;

namespace OpheliasOasis
{
	/// <summary>
	/// An enum representing the type of reservation.
	/// </summary>
	enum ReservationType
	{
		/// <summary>
		/// A prepaid reservation made 90 days in advance.
		/// </summary>
		Prepaid,

		/// <summary>
		/// A reservation placed 60 days in advance and paid for at least 45 days in advance
		/// </summary>
		SixtyDay,

		/// <summary>
		/// A conventional reservation.
		/// </summary>
		Conventional,

		/// <summary>
		/// A reservation offered at a discount to offset the low occupancy during those dates.
		/// </summary>
		Incentive
	};
}
