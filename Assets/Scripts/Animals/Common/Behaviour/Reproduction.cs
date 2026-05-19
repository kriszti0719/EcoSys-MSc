using Assets.Scripts.Animals.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
    private Animal animal;
    public Animal mate;
    public MateTraits mateTraits;
    public bool isPregnant = false;
    public int currentPregnancy;
    public int pregnancyDuration;
    void Start()
    {
        animal = GetComponent<Animal>();
        currentPregnancy = 0;
    }
    public void setReproduction(int _pregnancyDuration)
    {
        pregnancyDuration = _pregnancyDuration;
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
            DebugLogger.Error("AnimalSpawner not found in parent hierarchy for " + animal.name);
            return;
        }
        if (mateTraits == null)
        {
            if (mate != null)
            {
                mateTraits = new MateTraits(mate);
            }
            else
            {
                DebugLogger.Error("Mate wasn't saved. Can't randomize traits for the kids.");
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
        animal.gaveBirth++;
        animal.kids += rnd;
        isPregnant = false;
    }
    public bool IsPregnant()
    {
        return (isPregnant && currentPregnancy > 0);
    }
}
