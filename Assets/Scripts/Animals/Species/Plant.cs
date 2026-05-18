using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour, IEdible
{
    private int nutrition = 5;
    private int eatDuration = 5;
    public event Action OnConsumed;
    public int getNutrition()
    {
        return nutrition;
    }
    public void ToBeConsumed()
    {
        Invoke(nameof(Consumed), eatDuration);
    }
    private void Consumed()
    {
        OnConsumed?.Invoke();
        Destroy(gameObject);
    }
}
