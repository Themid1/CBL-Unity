using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Simulation simulation = new Simulation();

        Console.WriteLine("Enter simulation parameters (use , as separator):\n");

        // Get left mass
        Console.Write("Left  mass (kg): ");
        float leftMass = GetValidInput();

        // Get right mass
        Console.Write("Right mass (kg): ");
        float rightMass = GetValidInput();

        // Get reference angle
        Console.Write("Reference angle (rad): ");
        float refAngle = GetValidInput();

        // Get desired total mass
        Console.Write("Desired total mass (kg): ");
        float desiredMass = GetValidInput();

        // Set parameters
        simulation.SetParameters(leftMass, rightMass, refAngle, desiredMass);

        Console.WriteLine($"\n--- Parameters Set ---");
        Console.WriteLine($"Left: {leftMass} kg | Right: {rightMass} kg");
        Console.WriteLine($"Ref Angle: {refAngle} rad | Total Mass: {desiredMass} kg\n");

        Console.WriteLine("Commands: 'start' to begin, 'drain' to open all valves");
        Console.WriteLine("Type command:");

        while (true)
        {
            string command = Console.ReadLine()?.ToLower();

            if (command == "start")
            {
                // Run simulation
                simulation.Run();
                Console.WriteLine($"Stop Reason: {simulation.StopReason}");
                Console.WriteLine($"Final Left Mass: {simulation.LeftMass} kg");
                Console.WriteLine($"Final Right Mass: {simulation.RightMass} kg");
            }
            else if (command == "drain")
            {
                simulation.Drain();
            }
            else
            {
                Console.WriteLine("Unknown command. Type 'start' or 'drain'.");
            }
        }
    }

    /// Get valid float input from user, keep asking until valid input is provided
    static float GetValidInput()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (float.TryParse(input, out float result))
                return result;
            Console.Write("Invalid input. Please enter a valid number: ");
        }
    }
}
