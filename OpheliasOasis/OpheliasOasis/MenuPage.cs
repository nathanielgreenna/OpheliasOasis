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
    public class MenuPage : Page
    {
        private readonly List<Page> pages;

        public MenuPage(string title, string description, List<Page> pages) : base(title, description)
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
                int exitOption = pages.Count + 1;
                string selectionText;
                int selection;

                // Display menu options
                DisplayHeader();
                Console.WriteLine("Please select one of the following options:");
                for (int i = 0; i < pages.Count; i++)
                {
                    Console.WriteLine("\t" + (i + 1) + ": " + pages[i].getDescription());
                }
                Console.WriteLine("\t" + exitOption + ": Exit menu");
                Console.WriteLine();

                

                Console.Write("Please enter your selection (1-" + exitOption + "): ");
                selectionText = Console.ReadLine();

                // If necessary, repeatedly request a selection until a valid number is provided
                while (!int.TryParse(selectionText, out selection) || selection > exitOption || selection < 1)
                {
                    Console.Write("Selection \"" + selectionText + "\" is not valid. Please enter your selection (1-" + exitOption + "): ");
                    selectionText = Console.ReadLine();
                }

                Console.WriteLine();

                // Navigate to selected page
                if (selection == exitOption) return;
                else pages[selection - 1].Open();
            }
        }
    }
}
