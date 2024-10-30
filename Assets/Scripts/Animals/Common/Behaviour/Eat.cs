using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Eat : MonoBehaviour
    {
        private Animal animal;
        public HungerBar hungerBar;
        private int maxHunger = 100;
        private Plant food;
        public int starving;
        public int currentHunger;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void setBar(GameObject barsContainer)
        {
            this.hungerBar = barsContainer.GetComponentInChildren<HungerBar>();
            currentHunger = maxHunger;
            hungerBar.SetMaxHunger(maxHunger);
        }
        public void updateBar()
        {
            hungerBar.SetHunger(currentHunger);
        }
        public int getCurrentHunger()
        {
            return currentHunger;
        }
        public void Step()
        {
            currentHunger--;
        }
        public void StartEating()
        {
            food = animal.targetRef.GetComponent<Plant>();
            if (food != null)
            {

                animal.breakCounter = food.eatDuration;
                animal.status = Status.EATING;
            }
        }
        public void FinishEating()
        {
            if (food != null)
            {
                food.Die();
            }
            food = null;
        }
        public void Eating()
        {
            food.eatDuration--;
            if (currentHunger + food.nutrition < maxHunger || food.eatDuration >= 0)
                currentHunger += food.nutrition;
            else
            {
                currentHunger = maxHunger;
            }
        }
        public bool isFull()
        {
            return (currentHunger == maxHunger || food.eatDuration == 0);
        }
        public bool isStarving()
        {
            return currentHunger <= starving;
        }
        public bool isHungry()
        {
            return currentHunger < 70;
        }
        public bool isStarvedToDeath()
        {
            return currentHunger == 0;
        }
    }
}
