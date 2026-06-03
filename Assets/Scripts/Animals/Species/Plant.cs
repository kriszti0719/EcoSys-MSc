using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour, IEdible
{
    private int nutrition = 25;
    public int getNutrition()
    {
        return nutrition;
    }
    public void Consumed()
    {
        Destroy(gameObject);
    }
}
