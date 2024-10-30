using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirstBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMaxThirst(int thirst)
    {
        slider.maxValue = thirst;
        slider.value = thirst;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetThirst(int thirst)
    {
        slider.value = thirst;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
