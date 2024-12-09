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

        private int finished = 0;
        private int started = 0;
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
                finished = 0;
                if (started > 0)
                {
                    Debug.Log("started > 0: " + started);
                }
                started++;

                food = animal.targetRef.GetComponent<Plant>();
                if (food != null)
                {

                    animal.breakCounter = food.getEatDuration();
                    animal.status = Status.EAT;
                }
            }
            else if(animal.getTargetLayerToEat() == LayerMask.GetMask("Bunny"))
            {
                food = animal.targetRef.GetComponent<Bunny>();
                if (food != null)
                {

                    animal.breakCounter = food.getEatDuration();
                    animal.status = Status.EAT;
                    food.setStatusCaught();
                }
            }
        }
        public void FinishEating()
        {
            started = 0;
            if (food != null)
            {
                if(finished > 0) { Debug.Log("finished > 0"); }
                finished++;
                food.Eaten();
            }
            food = null;
        }
        public void Eating()
        {
            if (currentHunger + food.getNutrition() < maxHunger)
            {
                currentHunger += food.getNutrition();
            }
            else
            {
                currentHunger = maxHunger;
            }
        }
        public bool isFull()
        {
            if (currentHunger > maxHunger)
            {
                Debug.Log("RIP: currentHunger > maxHunger");
            }
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
