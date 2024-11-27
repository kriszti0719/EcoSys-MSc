using Assets.Scripts;
using Assets.Scripts.Animals.Species;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Plant : MonoBehaviour, Edible
{
    public int nutrition = 5;
    public int eatDuration = 10;
    public CauseOfDeath cause;

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
        cause = CauseOfDeath.EATEN;
        //this.GetComponentInParent<FoodSpawner>().RegisterDeath(this);
        Destroy(gameObject);
    }
}
