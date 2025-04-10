using System;
<<<<<<< Updated upstream
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
>>>>>>> Stashed changes
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Eat : MonoBehaviour
    {
        private Animal animal;
        public HungerBar hungerBar;
        private int maxHunger = 100;
<<<<<<< Updated upstream
        private Plant food;
        public int starving;
=======
        public IEdible food;
        public int critical;
>>>>>>> Stashed changes
        public int currentHunger;

        public event Action OnHungerCritical;
        public event Action OnHungerFull;
        public event Action OnHungerDepleted;

        void Start() {  animal = GetComponent<Animal>(); }
        private bool IsFull() => currentHunger == maxHunger;
        private bool IsCritical() => currentHunger <= critical;
        private bool IsDepleted() => currentHunger == 0;
        public bool IsHungry() => currentHunger < 70;
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
            if (animal.status == Status.EAT)
                currentHunger = Mathf.Min(currentHunger + food.getNutrition(), maxHunger);
            else
                currentHunger--;

            if (IsDepleted())
            {
                OnHungerDepleted?.Invoke();
            }
            else if (IsFull())
            {
                OnHungerFull?.Invoke(); 
            }
            else if (IsCritical())
            {
                OnHungerCritical?.Invoke();
            }
        }
        public void StartEating()
        {
<<<<<<< Updated upstream
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
=======
            food.AboutToBeConsumed();
        }
        public void FinishEating()
        {
            food.StopBeingConsumed();
            food = null;
        }
>>>>>>> Stashed changes
    }
}
