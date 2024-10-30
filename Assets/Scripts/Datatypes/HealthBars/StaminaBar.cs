using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetStamina(int stamina)
    {
        slider.value = stamina;

        //  Our value might go from 0 to 100
        //  But the gradient goes from 0 to 1
        //  ---> normalizedValue
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
