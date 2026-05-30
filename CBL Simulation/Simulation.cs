using System;
using System.Threading;

public class Simulation
{
    Tank left = new Tank();
    Tank right = new Tank();
    BallastSystem system = new BallastSystem();

    KalmanFilter kalman = new KalmanFilter();

    float dt = 0.1f; // Time step in seconds

    public void Run()
    {
        //Initial conditions
        left.Mass = 2;
        right.Mass = 2;

        while (true)
        {
            Step();
            Thread.Sleep(100); // Sleep for 100ms for readable console
        }
    }

    void Step()
    {
        // Get latest sensor data
        SensorData data = SensorData.GetLatest();

        //Kalman filter estimation
        var estimate = kalman.Update(data);

        // Update tanks with estimated masses
        left.UpdateMass(estimate.leftMass);
        right.UpdateMass(estimate.rightMass);

        // Update system angle based on new masses
        system.UpdateAngle(left.Mass, right.Mass);

        Console.Clear();

        Console.WriteLine($"Left Mass: {left.Mass}");
        Console.WriteLine($"Right Mass: {right.Mass}");
        Console.WriteLine($"Angle: {system.Angle}");

    }

}

