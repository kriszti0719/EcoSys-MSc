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
        [SerializeField]
        private int currentMatingUrge;
        public bool enableMating;
        public int matingCooldown = 0;
        private int maxMatingCooldown = 150;
        public MatingUrgeBar matingBar;
        private int maxMatingUrge = 100;
        [SerializeField]
        public int charm;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void setBar(GameObject barsContainer, bool randomize = false)
        {
            this.matingBar = barsContainer.GetComponentInChildren<MatingUrgeBar>();
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

            if (matingCooldown == 0 && animal.reproduction.isFertile && !animal.reproduction.isPregnant)
            {
                enableMating = true;
            }
            currentMatingUrge--;
        }
        public void Mating()
        {
            currentMatingUrge = maxMatingUrge;
            enableMating = false;
            matingCooldown = maxMatingCooldown;
            animal.triedForBaby++;
            
            IsSuccess();
            
            animal.targetRef = null;
            animal.sensor.targetMask = LayerMask.GetMask("None");
            (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
        }
        public bool IsAcceptable(Animal mate)
        {
            bool accepted = (mate.mating.charm + (100 - currentMatingUrge)) < charm;
            return accepted;
        }
        private void IsSuccess()
        {
            if (!animal.isMale)
            {
                // Base success rate for pregnancy
                float baseSuccessRate = 0.8f;
                // Random chance factor between -10% to +10%
                float randomChanceFactor = UnityEngine.Random.Range(-0.1f, 0.1f);
                // Calculate the overall success rate
                float overallSuccessRate = baseSuccessRate + randomChanceFactor;

                // Check if the mating is successful based on the calculated success rate
                animal.reproduction.isPregnant = UnityEngine.Random.value < overallSuccessRate;
                if (animal.reproduction.isPregnant)
                {
                    animal.reproduction.currentPregnancy = animal.reproduction.pregnancyDuration;
                }
                else
                    animal.reproduction.mate = null;
            }
        }
    }
}
