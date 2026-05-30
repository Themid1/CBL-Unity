using UnityEngine;

public class ControlSystemManager : MonoBehaviour
{
    public ArduinoReader arduinoReader;
    public ValveController valveController;
    public ControlSystemUI ui;

    private ControllerCalculator calculator;

    public float currentSensorValue;
    public float currentValveValue;
    public float currentTargetValue = 500;

    public float sensor1FlowValue;
    public float sensor2FlowValue;

    void Start()
    {
        calculator = new ControllerCalculator();
        calculator.SetTargetValue(currentTargetValue);

        if (ui != null)
        {
            ui.UpdateDisplay(
                sensor1FlowValue,
                sensor2FlowValue,
                currentValveValue,
                currentTargetValue
            );
        }
    }

    void Update()
    {
        if (arduinoReader == null || valveController == null || ui == null)
            return;

        if (arduinoReader.HasNewData)
        {
            sensor1FlowValue = arduinoReader.sensor1.flowMSec;
            sensor2FlowValue = arduinoReader.sensor2.flowMSec;
            
            // Debug.Log("DMM:sensor1FlowValue: " + sensor1FlowValue);

            currentSensorValue = arduinoReader.LatestSensorValue;

            currentValveValue = calculator.CalculateValveOutput(sensor1FlowValue, sensor2FlowValue);

            //need changing later
            valveController.SetValveOutput(sensor1FlowValue);

            ui.UpdateDisplay(
                sensor1FlowValue,
                sensor2FlowValue,
                currentValveValue,
                currentTargetValue
            );


            arduinoReader.ClearNewDataFlag();
        }
    }

    public void SetTargetValueFromUI(float newTargetValue)
    {
        currentTargetValue = newTargetValue;

        calculator.SetTargetValue(currentTargetValue);

        currentValveValue = calculator.CalculateValveOutput(sensor1FlowValue, sensor2FlowValue);

        //need changing later
        valveController.SetValveOutput(sensor1FlowValue);

        ui.UpdateDisplay(
            sensor1FlowValue,
            sensor2FlowValue,
            currentValveValue,
            currentTargetValue
        );

        if (arduinoReader != null)
        {
            arduinoReader.SendLineToArduino("SET_TARGET," + currentTargetValue);
        }
    }
}