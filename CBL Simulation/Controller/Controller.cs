using System;
public class Controller
{
    public float AngleError { get; private set; }
    public float MassError { get; private set; }
    public bool Finished { get; private set; }
    public string StopReason { get; set; }

    public ValveState Update(float thetaRef, float thetaEst, float leftMass, float rightMass, float mDesired)
    {
        // Tuning
        // angle treshold
        float epsilonAngle = 0.1f;
        // mass treshold (defined as 3 percent of desired mass)
        float epsilonMass = 0.03f * mDesired;

        AngleError = thetaRef - thetaEst;
        MassError = (leftMass + rightMass) - mDesired;

        ValveState valves = new ValveState();

        bool isAngleZero = Math.Abs(AngleError) < epsilonAngle;
        bool isMassZero = Math.Abs(MassError) < epsilonMass;

        // Safety stop: if either tank reaches 4.5 kg, close all valves and finish
        if (leftMass >= 4.5f || rightMass >= 4.5f)
        {
            Finished = true;
            StopReason = "Safety limit reached (4.5 kg)";
            return valves;
        }

        if (isAngleZero && isMassZero)
        {
            Finished = true;
            StopReason = "Target angle and mass reached";
        }

        if (MassError > epsilonMass) // em > 0 (Too heavy -> let water out)
        {
            if (AngleError > epsilonAngle) // etheta > 0 (Tilt left)
            {
                valves.V1 = false; valves.V2 = false; valves.V3 = false; valves.V4 = true;
            }
            else if (isAngleZero) // etheta = 0
            {
                valves.V1 = false; valves.V2 = true; valves.V3 = false; valves.V4 = true;
            }
            else if (AngleError < -epsilonAngle) // etheta < 0 (Tilt right)
            {
                valves.V1 = false; valves.V2 = true; valves.V3 = false; valves.V4 = false;
            }
        }
        else if (isMassZero) // em = 0 (Correct weight -> balance only)
        {
            if (AngleError > epsilonAngle) // etheta > 0 (Tilt left)
            {
                valves.V1 = true; valves.V2 = false; valves.V3 = false; valves.V4 = true;
            }
            else if (isAngleZero) // etheta = 0
            {
                valves.V1 = false; valves.V2 = false; valves.V3 = false; valves.V4 = false;
            }
            else if (AngleError < -epsilonAngle) // etheta < 0 (Tilt right)
            {
                valves.V1 = false; valves.V2 = true; valves.V3 = true; valves.V4 = false;
            }
        }
        else if (MassError < -epsilonMass) // em < 0 (Too light -> let water in)
        {
            if (AngleError > epsilonAngle) // etheta > 0 (Tilt left)
            {
                valves.V1 = true; valves.V2 = false; valves.V3 = false; valves.V4 = false;
            }
            else if (isAngleZero) // etheta = 0
            {
                valves.V1 = true; valves.V2 = false; valves.V3 = true; valves.V4 = false;
            }
            else if (AngleError < -epsilonAngle) // etheta < 0 (Tilt right)
            {
                valves.V1 = false; valves.V2 = false; valves.V3 = true; valves.V4 = false;
            }
        }


        return valves;
    }
}
