using Assets.Scripts;
using Assets.Scripts.Animals.Common.Behaviour;
using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour, Edible
{
    public int nutrition = 5;
    public int eatDuration = 10;
    public CauseOfDeath cause;
    public bool isDestroyed = false;

    private void Start()
    {
        nutrition = 5;
        eatDuration = 10;
        cause = CauseOfDeath.NONE;
    }
    public void Eat()
    {
        StartCoroutine(BeingEaten());
    }

    IEnumerator BeingEaten()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            eatDuration--;
        }
    } 

    public int getEatDuration()
    {
        return eatDuration;
    }

    public void setStatusCaught()
    {
        throw new NotImplementedException();
    }

    public int getNutrition()
    {
        return nutrition;
    }

    public void Eaten()
    {
        cause = CauseOfDeath.CONSUMED;

        if(!isDestroyed)
        {
            Destroy(this.gameObject);
        }
         isDestroyed = true;
    }
}
