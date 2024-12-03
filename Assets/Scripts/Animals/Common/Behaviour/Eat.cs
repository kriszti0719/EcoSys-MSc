using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Eat : MonoBehaviour
    {
        private Animal animal;
        public HungerBar hungerBar;
        private int maxHunger = 100;
        private Edible food;
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
            if (animal.getTargetLayerToEat() == LayerMask.GetMask("BunnyFood"))
            {
                food = animal.targetRef.GetComponent<Plant>();
                if (food != null)
                {

                    animal.breakCounter = food.getEatDuration();
                    animal.status = Status.EATING;
                }
            }
            else if(animal.getTargetLayerToEat() == LayerMask.GetMask("Bunny"))
            {
                food = animal.targetRef.GetComponent<Bunny>();
                if (food != null)
                {

                    animal.breakCounter = food.getEatDuration();
                    animal.status = Status.EATING;
                    food.setStatusCaught();
                }
            }
        }
        public void FinishEating()
        {
            if (food != null)
            {
                food.Eaten();
            }
            food = null;
        }
        public void Eating()
        {
            if (currentHunger + food.getNutrition() < maxHunger || animal.breakCounter >= 0)
                currentHunger += food.getNutrition();
            else
            {
                currentHunger = maxHunger;
            }
        }
        public bool isFull()
        {
            return (currentHunger == maxHunger || food.getEatDuration() == 0);
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
