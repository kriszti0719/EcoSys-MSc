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
    public void Destroy()
    {
        DebugLogger.RegisterDeath(step: animal.GetComponentInParent<AnimalSpawner>().getStep(), animal: animal);
        
        // For testing purposes only
        //DebugLogger.ShowNotification("Someone died :(");

        foreach (GameObject g in animal.destructibles)
        {
            Destroy(g);
        }
        Destroy(gameObject);
    }
    public void CatchPrey()
    {
        isCaptured = true;
    }
}
