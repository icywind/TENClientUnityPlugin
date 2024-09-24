using UnityEngine;

public class SphereVisualizer : MonoBehaviour
{
    [SerializeField]
    float sizeMultiplier = 10f; // Multiplier for sphere size
    float[] _pcmData = new float[0];

    private void Update()
    {
        if (_pcmData.Length > 0)
        {
            UpdateDisplay();
        }
    }


    public void UpdateVisualizer(float[] pcmData)
    {
        _pcmData = pcmData;
    }

    void UpdateDisplay()
    {
        float sum = 0f;

        // Calculate the sum of the absolute values of the PCM data
        foreach (float sample in _pcmData)
        {
            sum += Mathf.Abs(sample);
        }

        // Calculate the average energy
        float average = sum / _pcmData.Length;

        // Adjust the size of the sphere based on the average energy
        float newSize = 1f + average * sizeMultiplier;
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }
}
