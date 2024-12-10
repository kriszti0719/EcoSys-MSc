using Assets.Scripts.Datatypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
    private Animal animal;
    public Animal mate;
    public MateTraits mateTraits;
    public bool isPregnant = false;

    public bool isFertile;
    public int reproductiveUrge;        // higher chance to reproduction BUT spends more time finding a mate
    public int currentPregnancy;
    public int pregnancyDuration;  // stronger kids BUT can die during pregnancy meaning no inheriting at all
    void Start()
    {
        animal = GetComponent<Animal>();
    }
    public void setReproduction(int _reproductiveUrge, int _pregnancyDuration, bool _isFertile)
    {
        this.reproductiveUrge = _reproductiveUrge;
        this.pregnancyDuration = _pregnancyDuration;
        this.isFertile = _isFertile;
        this.currentPregnancy = 0;
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
        var spawner = animal.GetComponentInParent<AnimalSpawner>();
        if (spawner == null)
        {
            Debug.LogError("AnimalSpawner not found in parent hierarchy for " + animal.name);
            return;
        }
        if (mateTraits == null)
        {
            //throw new System.Exception("FatherTraits is null");
            if(mate != null)
            {
                mateTraits = new MateTraits(mate);
            }
            else
            {
                return;
            }

        }
        int rnd = animal.species switch
        {
            Species.FOX => Random.Range(4, 7),
            Species.BUNNY => Random.Range(6, 9),
            _ => throw new System.ArgumentException($"Unhandled species: {animal.species}")
        };

        spawner.SpawnBabies(null, rnd, animal, mateTraits);
        isPregnant = false;
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
