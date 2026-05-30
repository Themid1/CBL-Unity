public class Tank
{

    // Physical variables
    // public string Name;
    public float Density = 1000f; // water density

    // State variable
    public float Mass; // What evolves

    //// Flow rates 
    //public float Inflow;
    //public float Outflow;

    // Volume
    public float Volume => Mass / Density;

    // Dynamics
    public void UpdateMass(float estimatedMass)
    {
        Mass = Math.Max(0, estimatedMass);

    }
}
