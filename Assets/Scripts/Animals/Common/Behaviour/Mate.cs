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
        public int Charm;
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
            enableMating = false;
        }
        public void ToMate()
        {
            animal.breakCounter = mateDuration;
            animal.prevStatus = animal.status;
            animal.status = Status.MATE;
        }
        public bool isAcceptable(Animal mate)
        {
            //int charmDifference = mate.charm - charm; // Jó eséllyel pozitív, de lehet - is
            //float matingUrgeDifference = currentMatingUrge / 100f;
            //float acceptanceChance = Mathf.Clamp01(0.5f + charmDifference * 0.01f + matingUrgeDifference); 
            //float randomValue = Random.value;

            bool accepted = (mate.mating.Charm + (100 - currentMatingUrge)) < Charm;

            if (accepted)
            {
                bool canSee = animal.sensor.FieldOfViewCheck(animal.getTargetLayerToMate(), mate.transform.gameObject);
                animal.prevStatus = Status.SEARCH_MATE;

                if (!canSee)
                {
                    animal.sensor.targetMask = LayerMask.GetMask("None");
                    animal.targetRef = null;
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
        public void isSuccess()
        {
            if (!animal.isMale)     //TODO:  && !isPregnant
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
