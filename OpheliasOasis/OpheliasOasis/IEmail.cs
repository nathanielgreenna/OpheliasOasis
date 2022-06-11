/*
 * IEmail
 * 
 * Description: Defines the information associated with an email object.
 * 
 * Changelog:
 * 4/20/2022: Initial code - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
	/// <summary>
	/// An interface to hold the information associated with an email.
	/// </summary>
	interface IEmail
	{
		/// <summary>
		/// Get a list of recipient emails.
		/// </summary>
		/// <returns>A list of strings, each containing the email address of a recipient.</returns>
		public List<string> GetRecipients();

		/// <summary>
		/// Get the header text.
		/// </summary>
		/// <returns>A string containing the header text.</returns>
		public string GetHeaderText();

		/// <summary>
		/// Get the body text.
		/// </summary>
		/// <returns>A string containing the body text.</returns>
		public string GetBodyText();

		/// <summary>
		/// Get the list of attatchment file paths.
		/// </summary>
		/// <returns>A list of strings, each representing a file path to an attachment.</returns>
		public List<string> GetAttachments();
	}
}
