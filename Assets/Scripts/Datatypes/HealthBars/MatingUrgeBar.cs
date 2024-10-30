using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatingUrgeBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMaxMatingUrge(int matingUrge)
    {
        slider.maxValue = matingUrge;
        slider.value = matingUrge;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetMatingUrge(int matingUrge)
    {
        slider.value = matingUrge;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
