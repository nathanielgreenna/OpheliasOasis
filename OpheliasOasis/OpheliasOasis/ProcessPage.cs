/*
 * Page
 * 
 * Description: An extension of the page class that provides facilities for moving back and forth between multiple steps.
 * 
 * Changelog:
 * 4/22/2022: Initial code - Alex
 * 4/22/2022: Changed navigation options to Quit and Back, changes constructor to accept stepDescs and an endStep - Nathan
 * 4/23/2022: Updated documentation. Changed stepDescs to prompts and endStep to saveChanges. Changed exit prompts and default indicators. Changed string to work with $. - Alex
 */

using System;
using System.Collections.Generic;

namespace OpheliasOasis
{
    /// <summary>
    /// A class to provide a page that walks the user through a process. Allows for forward and backward navigation.
    /// It accepts a list of functions. Each fuction accepts the input from the propmt and returns an error message.
    /// If the error message is blank, it is assumed
    /// </summary>
	public class ProcessPage : Page
	{
        private readonly List<Tuple<Func<String, String>, String>> steps;
        private readonly Func<String> saveChanges;

        /// <summary>
        /// Create a new process page using the steps provided.
        /// </summary>
        /// <param name="title">A String containing the page title.</param>
        /// <param name="description">A String containing the page description.</param>
        /// <param name="prompts">A list of Strings, each containing a prompt for the respective step.</param>
        /// <param name="steps">A list of functions, each accepting an input String and returning an error message.</param>
        /// <param name="saveChanges">A function that runs before exiting.</param>
        public ProcessPage(String title, String description, List<Tuple<Func<String, String>, String>> steps, Func<String> saveChanges) : base(title, description)
        {
            // Store input
            this.steps = steps;
            this.saveChanges = saveChanges;
        }

        /// <summary>
        /// Run through the steps provided when the object was intantiated
        /// </summary>
        public override void Open()
        {
            // Store current step and input
            int step = 1;
            string uInput;
            string procOutput;

            // Display page information
            DisplayHeader();
            DisplayStepNavigationHint();

            // Loop through the steps - allows user to move backwards and forwards through the process
            while (step <= steps.Count)
            {
                // Aquire user input
                Console.Write($"[Step {step} of {steps.Count}] {steps[step - 1].Item2}: ");
                uInput = Console.ReadLine();
                // Handle user input
                switch (uInput.ToUpper())
                {
                    // Back
                    case "B":
                        if (step > 1)
                        {
                            step--;
                        }
                        else
                        {
                            Console.Write("Exit without saving? All progress will be lost. (y/N): ");
                            if (Console.ReadLine().ToUpper() == "Y") return;
                        }
                        break;

                    // Quit
                    case "Q":
                        Console.Write("Exit without saving? All progress will be lost. (y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y") return;
                        break;

                    // Read in response and move on to the next step
                    default:
                        procOutput = steps[step - 1].Item1.Invoke(uInput);
                        if (String.IsNullOrEmpty(procOutput))
                        {
                            if (step < steps.Count)
                            {
                                step++;
                            }
                            else
                            {
                                Console.Write("Save and exit? (Y/n): ");

                                if (Console.ReadLine().ToUpper() != "N")
                                {
                                    if(saveChanges != null)
                                    {
                                        procOutput = saveChanges.Invoke();
                                        if (String.IsNullOrEmpty(procOutput))
                                        {
                                            return;
                                        }
                                        else {
                                            Console.WriteLine($"Error: {procOutput}. Please try again.");
                                            break;
                                        }
                                        
                                    }
                                    
                                    return;
                                }
                            }
                        }
                        else 
                        {
                            Console.WriteLine($"Error: {procOutput}. Please try again.");
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
