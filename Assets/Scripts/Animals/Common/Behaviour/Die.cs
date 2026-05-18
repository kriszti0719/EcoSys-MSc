using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Die : MonoBehaviour
{
    private Animal animal;
    public bool isCaptured = false;

    void Start()
    {
        animal = GetComponent<Animal>();
    }
    public void DestroyAnimal()
    {
        //DebugLogger.ShowNotification("Someone died :(");
        animal.GetComponentInParent<AnimalSpawner>().RemoveAnimal(animal);

        foreach (GameObject g in animal.destructibles)
        {
            Destroy(g);
        }
        Destroy(gameObject);
    }
    public void HandleDeath(CauseOfDeath cause)
    {
        if (animal.status != Status.DIE)
        {
            animal.cause = cause;
            (animal.prevStatus, animal.status) = (animal.status, Status.DIE);
            DestroyAnimal();
        }
    }
}
