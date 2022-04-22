/*
 * Page
 * 
 * Description: An extension of the page class that provides facilities for moving back and forth between multiple steps.
 * 
 * Changelog:
 * 4/22/2022: Initial code - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
	public class ProcessPage : Page
	{
		private readonly List<Action> steps;

        public ProcessPage(string title, string description, List<Action> steps) : base(title, description)
		{
			this.steps = steps;
		}

        /// <summary>
        /// Run through the steps provided when the object was intantiated
        /// </summary>
        public override void Open()
        {

            // Store step variables
            int step = 1;

            // Display page information
            DisplayHeader();
            DisplayStepNavigationHint();

            // Loop through the steps - allows user to move backwards and forwards through the process
            while (step <= steps.Count)
            {
                // Keep user informed of progress
                Console.Write("Step " + step + " of " + steps.Count + ": ");

                // Perform step
                steps[step - 1].Invoke();

                // Navigate to the next user-selected step
                Console.Write("Continute? (Enter B, R, C):");

                switch (Console.ReadLine().ToUpper())
                {
                    case "B":
                        if (step > 1)
                        {
                            step--;
                        }
                        else
                        {
                            Console.Write("Are you sure you want to exit this menu? (Y/n):");
                            if (Console.ReadLine().ToUpper() != "N") return;
                        }
                        break;
                    case "R":
                        break;
                    default:
                        if (step < steps.Count)
                        {
                            step++;
                        }
                        else
                        {
                            Console.Write("Are you sure you want to exit this menu? (Y/n):");
                            if (Console.ReadLine().ToUpper() != "N") return;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Display a reminder for how to navigate multi-step processes.
        /// </summary>
        private static void DisplayStepNavigationHint()
        {
            Console.WriteLine("Reminder - enter the following between steps:");
            Console.WriteLine("\tB: Move back a step");
            Console.WriteLine("\tR: Repeat the current step");
            Console.WriteLine("\tC (default): Continue to the next step.");
            Console.WriteLine();
        }
    }
}
