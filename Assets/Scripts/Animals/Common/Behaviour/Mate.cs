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
        private int mateDuration = 1;
        public MatingUrgeBar matingBar;
        private int maxMatingUrge = 100;
        [SerializeField]
        public int charm;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void setBar(GameObject barsContainer)
        {
            this.matingBar = barsContainer.GetComponentInChildren<MatingUrgeBar>();
            currentMatingUrge = maxMatingUrge;
            matingBar.SetMaxMatingUrge(maxMatingUrge);
        }
        public void updateBar()
        {
            matingBar.SetMatingUrge(currentMatingUrge);
        }
        public void Step()
        {
            currentMatingUrge--;
        }
        public void Mating()
        {
            if (currentMatingUrge != maxMatingUrge)
                currentMatingUrge = maxMatingUrge;
        }
        public void ToMate()
        {
            animal.breakCounter = mateDuration;
            animal.prevStatus = animal.status;
            animal.status = Status.MATE;
        }
        public bool isAcceptable(Animal mate)
        {
            if(animal.reproduction.isPregnant |
                (animal.reproduction.currentPregnancy > 0) |
                !animal.reproduction.isFertile |
                !enableMating)
            {
                return false;
            }
            if(animal.status == Status.FLEE |
                animal.status == Status.WAIT |
                animal.status == Status.MATE |
                animal.status == Status.DIE |
                animal.status == Status.CAUGHT)
            {
                return false;
            }
            //bool accepted = (mate.mating.charm + (100 - currentMatingUrge)) < charm;
            bool accepted = true;

            if (accepted)
            {
                if(animal.status == Status.EAT | animal.status == Status.DRINK)
                {
                    animal.prevStatus = animal.status;
                    animal.status = Status.WAIT;
                    return accepted;
                }

                animal.prevStatus = Status.SEARCH_MATE;
                bool canSee = animal.sensor.FieldOfViewCheck(animal.getTargetLayerToMate(), mate.transform.gameObject);

                if (!canSee)
                {
                    animal.sensor.targetMask = LayerMask.GetMask("None");
                    animal.targetRef = mate.transform.gameObject;
                    animal.status = Status.WAIT;
                }
                else
                {
                    animal.setTargetLayerToMate();
                    animal.targetRef = mate.transform.gameObject;
                    animal.status = Status.MOVE_TOWARDS;
                }
            }
            return accepted;
        }
        public void Success()
        {
            if (!animal.isMale && !animal.reproduction.isPregnant && !(animal.reproduction.currentPregnancy > 0))     //TODO:  && !isPregnant
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
                    enableMating = false;
                    animal.reproduction.currentPregnancy = animal.reproduction.pregnancyDuration;
                    if (animal.reproduction.mate == null)
                    {
                        throw new Exception("Hehe");
                    }
                }
                else
                {
                    animal.reproduction.mate.reproduction.mate = null;
                    animal.reproduction.mate.reproduction.mateTraits = null;
                    animal.reproduction.mate = null;
                    animal.reproduction.mateTraits = null;
                    if(animal.reproduction.currentPregnancy != 0 || animal.reproduction.isPregnant)
                    {
                        throw new Exception("Hhehe");
                    }
                }
            }
        }
    }
}
