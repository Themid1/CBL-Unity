class Program
{
    static void Main()
    {
        Simulation simulation = new Simulation();
        Console.WriteLine("=== Simulation Ready ===");
        Console.WriteLine("Commands: 'start' to begin, 'drain' to open valves");
        Console.WriteLine("Type command:");

        while (true)
        {
            string command = Console.ReadLine()?.ToLower();

            if (command == "start")
            {
                simulation.Run();
            }
            else if (command == "drain")
            {
                simulation.Drain();
                Console.WriteLine("Valves opened. Tanks draining. Type 'start' to begin simulation.");
            }
            else
            {
                Console.WriteLine("Unknown command. Type 'start' or 'drain'");
            }
        }
        
    }
}
