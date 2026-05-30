using UnityEngine;
using TMPro;

public class ControlSystemUI : MonoBehaviour
{
    public TMP_Text sensor1Text;
    public TMP_Text sensor2Text;
    public TMP_Text valveText;
    public TMP_Text targetText;

    public TMP_InputField targetInput;

    public ControlSystemManager controlSystemManager;

    public void UpdateDisplay(
        float sensor1Flow,
        float sensor2Flow,
        float valveValue,
        float targetValue
    )
    {
        sensor1Text.text = "Sensor 1 Flow: " + sensor1Flow.ToString("F2") + " m^3/sec";
        sensor2Text.text = "Sensor 2 Flow: " + sensor2Flow.ToString("F2") + " m^3/sec";
        valveText.text = "Valve Status: " + valveValue.ToString("F2");
        targetText.text = "Target Value: " + targetValue.ToString("F2");
    }

    public void SubmitTargetValue()
    {
        if (targetInput == null || controlSystemManager == null)
            return;

        string inputText = targetInput.text;

        if (float.TryParse(inputText, out float newTargetValue))
        {
            controlSystemManager.SetTargetValueFromUI(newTargetValue);
        }
        else
        {
            Debug.LogWarning("Invalid target value input.");
        }
    }
}