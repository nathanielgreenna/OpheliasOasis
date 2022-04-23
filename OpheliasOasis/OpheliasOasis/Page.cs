/*
 * Page
 * 
 * Description: An abstract class to represent a page in the UI.
 * 
 * Changelog:
 * 4/22/2022: Initial code - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
    /// <summary>
    /// Represent a page in the user interface.
    /// </summary>
	public abstract class Page
	{
        private readonly string title;
        private readonly string description;

        /// <summary>
        /// Create a new page with the specified title.
        /// </summary>
        /// <param name="title">The title of the new page.</param>
        public Page(string title, string description)
		{
            this.title = title;
            this.description = description;
		}

        /// <summary>
        /// Open the page (specifices are determined by the overrding class).
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Get the description of the page. Used for page headers and menu options.
        /// </summary>
        /// <returns>A string containing the description of the page.</returns>
        public string getDescription()
        {
            return description;
        }

        /// <summary>
        /// Get the title of the page. Used for page headers and menu options.
        /// </summary>
        /// <returns>A string containing the description of the page.</returns>
        public string getTitle()
        {
            return title;
        }





        /// <summary>
        /// Clear the screen and display a header for the current page.
        /// </summary>
        public void DisplayHeader()
        {
            Console.Clear();
            Console.WriteLine("====================| " + title.ToUpper() + " |====================");
            Console.WriteLine(description);
            Console.WriteLine();
        }
    }
}
