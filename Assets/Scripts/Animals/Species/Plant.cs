using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    {
        cause = CauseOfDeath.EATEN;
        //this.GetComponentInParent<FoodSpawner>().RegisterDeath(this);
        Destroy(gameObject);
    }
}
