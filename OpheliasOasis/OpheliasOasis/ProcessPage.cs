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
		private readonly List<Func<String, Boolean>> steps;
        private readonly List<String> stepDescs;
        private readonly Action endstep;

        public ProcessPage(string title, string description, List<String> stepDescs,List<Func<String, Boolean>> steps, Action endstep) : base(title, description)
		{
			this.steps = steps;
            this.stepDescs = stepDescs;
            this.endstep = endstep;
		}

        /// <summary>
        /// Run through the steps provided when the object was intantiated
        /// </summary>
        public override void Open()
        {
            String uInput;
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

                // Navigate to the next user-selected step
                Console.Write( stepDescs[step-1]);
                uInput = Console.ReadLine();

                switch (uInput.ToUpper())
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
                    case "Q":
                        Console.Write("Confirm quit (Y/n):");
                        if (Console.ReadLine().ToUpper() == "Y") return;
                        break;

                    default:
                        if (steps[step - 1].Invoke(uInput))
                        {
                            if (step < steps.Count)
                            {
                                step++;
                            }
                            else
                            {
                                Console.Write("Confirm (Y/n):");

                                if (Console.ReadLine().ToUpper() != "N")
                                {
                                    if(endstep != null)
                                    {
                                        endstep.Invoke();
                                    }
                                    
                                    return;
                                }
                            }
                            
                        }
                        else 
                        {
                            Console.WriteLine("Invalid input. Try again.");
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
            Console.WriteLine("Special input commands:");
            Console.WriteLine("\tB: Move back a step");
            Console.WriteLine("\tQ: Quit this screen");
            Console.WriteLine();
        }
    }
}
