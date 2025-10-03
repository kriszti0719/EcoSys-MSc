using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour, IEdible
{
    private int nutrition = 5;
    private int eatDuration = 10;
    private int currentDuration;
    public CauseOfDeath cause = CauseOfDeath.NONE;
    public void Start()
    {
        currentDuration = eatDuration;
    }
    public event Action OnConsumed;
    public int getNutrition()
    {
        return nutrition;
    }
    public void AboutToBeConsumed()
    {
        cause = CauseOfDeath.CONSUMED;
        StartCoroutine(ToBeConsumed());
    }
    public IEnumerator ToBeConsumed()
    {
        while (currentDuration > 0)
        {
            if (OnConsumed != null)
            {
                yield return new WaitForSeconds(1f);
                currentDuration--;
            }
            else
            {
                yield break;
            }
        }
        OnConsumed?.Invoke();
        Consumed();
    }
    private void Consumed()
    {
        //this.GetComponentInParent<FoodSpawner>().RegisterDeath(this);
        Destroy(gameObject);
    }
}
