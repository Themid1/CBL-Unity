using System;

public class SensorData
{
    public float LeftFlowIn;
    public float LeftFlowOut;

    public float RightFlowIn;
    public float RightFlowOut;

    public float LeftPressure;
    public float RightPressure;

    private static Random rand = new Random();

    // Fake sensor generator
    public static SensorData GetLatest(ValveState valves)
    {
        // Flow in m³/s (SI units). Base flow: 0.0001 m³/s = 0.1 L/s
        float FlowIn = 0.0001f;
        float FlowOut = 0.0003f;

        // Multipliers
        float leftInMult = valves.V1 ? 1f : 0f;
        float leftOutMult = valves.V2 ? 1f : 0f;
        float rightInMult = valves.V3 ? 1f : 0f;
        float rightOutMult = valves.V4 ? 1f : 0f;

        return new SensorData
        {

            LeftFlowIn = FlowIn * leftInMult,
            LeftFlowOut = FlowOut * leftOutMult,
            RightFlowIn = FlowIn * rightInMult,
            RightFlowOut = FlowOut * rightOutMult,

            LeftPressure = 218f + (float)(rand.NextDouble() * 40 - 20),
            RightPressure = 218f + (float)(rand.NextDouble() * 40 - 20),
        };
    }
}
