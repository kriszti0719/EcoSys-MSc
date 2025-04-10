using System;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Eat : MonoBehaviour
    {
        private Animal animal;
        public HungerBar hungerBar;
        private int maxHunger = 100;
        public IEdible food;
        public int critical;
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
            food.AboutToBeConsumed();
        }
        public void FinishEating()
        {
            food.StopBeingConsumed();
            food = null;
        }
    }
}
