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
    public static SensorData GetLatest()
    {
        return new SensorData
        {
            // base flow + noise
            LeftFlowIn = 2f + (float)(rand.NextDouble() * 0.2 - 0.1),
            LeftFlowOut = 0.5f + (float)(rand.NextDouble() * 0.2 - 0.1),

            RightFlowIn = 0,
            RightFlowOut = 0.5f + (float)(rand.NextDouble() * 0.2 - 0.1),

            // base pressure + noise
            LeftPressure = 218f
            + (float)(rand.NextDouble() * 40 - 20),  

            RightPressure = 218f
            + (float)(rand.NextDouble() * 40 - 20)  
        };
    }
}
