using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Mate : MonoBehaviour
    {
        private Animal animal;
        [HideInInspector] public MatingUrgeBar matingBar;
        public bool enableMating;
        public int charm;
        public int maxMatingUrge = 100;
        public int currentMatingUrge;
        public int maxMatingCooldown = 300;
        public int matingCooldown = 0;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void setBar(GameObject barsContainer, bool randomize = false)
        {
            matingBar = barsContainer.GetComponentInChildren<MatingUrgeBar>();
            currentMatingUrge = randomize ? UnityEngine.Random.Range(40, maxMatingUrge) : maxMatingUrge;
            if (randomize)
            {
                matingCooldown = UnityEngine.Random.Range(0, maxMatingCooldown);
                enableMating = matingCooldown == 0;
            }
            matingBar.SetMaxMatingUrge(maxMatingUrge);
        }
        public void updateBar()
        {
            matingBar.SetMatingUrge(currentMatingUrge);
        }
        public void Step()
        {
            if (matingCooldown > 0)
            {
                matingCooldown--;
            }

            if (matingCooldown == 0 && animal.aging.currentAge >= 1 && !animal.reproduction.isPregnant)
            {
                enableMating = true;
            }
            currentMatingUrge--;
        }
        public void Mating(Animal mate)
        {
            currentMatingUrge = maxMatingUrge;
            enableMating = false;
            matingCooldown = maxMatingCooldown;
            animal.triedForBaby++;
            
            IsSuccess(mate);
            
            animal.targetRef = null;
            animal.sensor.targetMask = LayerMask.GetMask("None");
            (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
        }
        public bool IsAcceptable(Animal mate)
        {
            bool accepted = (mate.mating.charm + (100 - currentMatingUrge)) < charm;
            return accepted;
        }
        private void IsSuccess(Animal mate)
        {
            if (!animal.isMale)
            {
                float baseSuccessRate = 0.8f;
                float randomChanceFactor = UnityEngine.Random.Range(-0.1f, 0.1f);
                float overallSuccessRate = baseSuccessRate + randomChanceFactor;

                animal.reproduction.isPregnant = UnityEngine.Random.value < overallSuccessRate;
                if (animal.reproduction.isPregnant)
                {
                    animal.reproduction.currentPregnancy = animal.reproduction.pregnancyDuration;
                    animal.reproduction.mateTraits = new MateTraits(mate);
                    animal.reproduction.mate = mate;
                }
                else
                    animal.reproduction.mate = null;
            }
        }
    }
}
