using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

<<<<<<< Updated upstream
public class Plant : MonoBehaviour
{
    public int nutrition = 5;
    public int eatDuration = 10;
    public CauseOfDeath cause;

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
    public void Die()
=======
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
>>>>>>> Stashed changes
    {
        //this.GetComponentInParent<FoodSpawner>().RegisterDeath(this);
        Destroy(gameObject);
    }
}
