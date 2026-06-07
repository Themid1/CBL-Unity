using System;
using System.Threading;

public class Simulation
{
    Tank left = new Tank();
    Tank right = new Tank();
    BallastSystem system = new BallastSystem();

    KalmanFilter kalman;
    Controller controller = new Controller();
    ValveState valves = new ValveState();

    // Default Simulation parameters
    private float leftMassInitial = 2f;
    private float rightMassInitial = 2f;
    private float referenceAngle = 0.26f;
    private float desiredTotalMass = 4f;

    // Public properties to access current state
    public float LeftMass => left.Mass;
    public float RightMass => right.Mass;
    public bool IsFinished => controller.Finished;
    public string StopReason => controller.StopReason;

    /// Set simulation parameters before running
    public void SetParameters(float leftMass, float rightMass, float refAngle, float desiredMass)
    {
        leftMassInitial = leftMass;
        rightMassInitial = rightMass;
        referenceAngle = refAngle;
        desiredTotalMass = desiredMass;
        
        // Initialize Kalman filter with initial values
        kalman = new KalmanFilter(initialLeftMass: leftMassInitial, initialRightMass: rightMassInitial, pressureVariance: 1e10, flowVariance: 0);
    }

    /// Run the simulation loop
    public void Run()
    {
        // Initialize tank masses
        left.Mass = leftMassInitial;
        right.Mass = rightMassInitial;

        while (!controller.Finished)
        {
            // Sample sensor data at the beginning of each cycle
            SensorData data = SensorData.GetLatest(valves);

            Step(data);

            // Check for quit command
            if (Console.KeyAvailable && Console.ReadKey(true).KeyChar == 'q')
            {
                // Close all valves on quit
                valves.V1 = false;
                valves.V2 = false;
                valves.V3 = false;
                valves.V4 = false;

                Console.WriteLine("\nExiting simulation. All valves closed.");
                controller.StopReason = "Simulation stopped by user";
                break;
            }

            Thread.Sleep(100);
        }
    }

    /// Drain mode: Opens all output valves and displays state in real-time
    /// Press 'q' to exit drain mode
    public void Drain()
    {
        valves.V1 = true;
        valves.V2 = true;  
        valves.V3 = true;
        valves.V4 = true;

        while (true)
        {
            // Get real-time sensor data
            SensorData data = SensorData.GetLatest(valves);

            // Update mass estimates
            Update(data);

            // Display state without errors (cleaner view during drain)
            DisplayState(showErrors: false);

            // Check for exit command
            if (Console.KeyAvailable && Console.ReadKey(true).KeyChar == 'q')
            {
                // Close all valves on exit
                valves.V1 = false;
                valves.V2 = false;
                valves.V3 = false;
                valves.V4 = false;

                Console.WriteLine("\nExiting drain mode. All valves closed.");
                break;
            }

            Thread.Sleep(100);
        }
    }

    void Step(SensorData data)
    {
        Update(data);

        // Update system angle based on new masses
        system.UpdateAngle(left.Mass, right.Mass);

        float EstimatedAngle = system.Angle;

        valves = controller.Update(referenceAngle, EstimatedAngle, left.Mass, right.Mass, desiredTotalMass);

        DisplayState(showErrors: true);
    }

    void Update(SensorData data)
    {
        // Update Kalman filter with sensor data
        var estimate = kalman.Update(data);

        // Update tank masses from sensor estimates
        left.UpdateMass(estimate.leftMass);
        right.UpdateMass(estimate.rightMass);
    }

    /// Display current simulation state (masses, valves, and optionally errors)
    void DisplayState(bool showErrors = false)
    {
        Console.Clear();
        // Always show masses and valve states
        Console.WriteLine("Process running. Press 'q' to quit.\n");
        Console.WriteLine($"Left Mass: {left.Mass:F3} kg");
        Console.WriteLine($"Right Mass: {right.Mass:F3} kg");
        Console.WriteLine($"V1: {valves.V1} | V3: {valves.V3}");
        Console.WriteLine($"V2: {valves.V2} | V4: {valves.V4}");

        // Optionally show errors
        if (showErrors)
        {
            Console.WriteLine($"Angle Error: {controller.AngleError:F3}");
            Console.WriteLine($"Mass Error: {controller.MassError:F3}");
        }
        
    }
}