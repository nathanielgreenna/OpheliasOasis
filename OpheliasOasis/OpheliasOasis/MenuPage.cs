/*
 * MenuPage
 * 
 * Description: An extension of page with facilities for displaying menu items.
 * 
 * Changelog:
 * 4/22/2022: Initial code - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
    /// <summary>
    /// A page with utilities for menus.
    /// </summary>
    public class MenuPage : Page
    {
        private readonly List<Page> pages;

        /// <summary>
        /// creates a new menu pages that opens the pages specified.
        /// </summary>
        /// <param name="title">A string containing the title of the page.</param>
        /// <param name="description">A string containing a description of the page.</param>
        /// <param name="pages">An ordered list of pages to transition to</param>
        public MenuPage(String title, String description, List<Page> pages) : base(title, description)
        {
            this.pages = pages;
        }

        /// <summary>
        /// Display the menu options and request an input. The navigate to the selected page.
        /// </summary>
        public override void Open()
        {
            // Do not exit the menu unless requested by the user
            while (true)
            {
                // Store I/O information
                string selectionText;
                int selection;

                // Display menu options
                DisplayHeader();
                Console.WriteLine("Please select one of the following options:");
                Console.WriteLine("\t0: Exit menu");
                for (int i = 0; i < pages.Count; i++)
                {
                    Console.WriteLine("\t" + (i + 1) + ": " + pages[i].GetTitle());
                }
                Console.WriteLine();

                Console.Write("Please enter your selection (0 - " + pages.Count + "): ");
                selectionText = Console.ReadLine();

                // If necessary, repeatedly request a selection until a valid number is provided
                while (!int.TryParse(selectionText, out selection) || selection > pages.Count || selection < 0)
                {
                    Console.Write("Selection \"" + selectionText + "\" is not valid. Please enter your selection (0 - " + pages.Count + "): ");
                    selectionText = Console.ReadLine();
                }

                Console.WriteLine();

                // Navigate to selected page
                if (selection == 0) return;
                else pages[selection - 1].Open();
            }
        }
    }
}
