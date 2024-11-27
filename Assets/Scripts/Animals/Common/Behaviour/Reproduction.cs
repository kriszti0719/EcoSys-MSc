using Assets.Scripts.Datatypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
    private Animal animal;
    public Animal mate;
    public bool isPregnant = false;

    public bool isFertile;
    public int reproductiveUrge;        // higher chance to reproduction BUT spends more time finding a mate
    public int currentPregnancy;
    public int pregnancyDuration;  // stronger kids BUT can die during pregnancy meaning no inheriting at all
    void Start()
    {
        animal = GetComponent<Animal>();
        currentPregnancy = 0;
    }
    public void setReproduction(int _reproductiveUrge, int _pregnancyDuration, bool _isFertile)
    {
        this.reproductiveUrge = _reproductiveUrge;
        this.pregnancyDuration = _pregnancyDuration;
        this.isFertile = _isFertile;
    }
    public void StepPregnancy()
    {
        currentPregnancy--;
        if (currentPregnancy == 0)
        {
            GiveBirth();
        }
    }
    public void GiveBirth()
    {
        isPregnant = false;
        //animal.GetComponentInParent<AnimalSpawner>().SpawnAnimals(null, Random.Range(6, 9), animal, animal.reproduction.mate);
    }
    public void Mature()
    {
        if (!isFertile)
        {
            isFertile = true;
        }
    }
    public bool isStillPregnant()
    {
        return (isPregnant && currentPregnancy > 0);
    }
}
