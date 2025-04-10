using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Drink : MonoBehaviour
    {
        private Animal animal;
        public ThirstBar thirstBar;
        private int drinkDuration = 5;
        private int drinkAmount = 10;
        private int maxThirst = 200;
        public int currentThirst;
        public int critical;

        public event Action OnThirstCritical;
        public event Action OnThirstFull;
        public event Action OnThirstDepleted;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        private bool IsFull() => currentThirst == maxThirst;
        private bool IsCritical() => currentThirst <= critical;
        private bool IsDepleted() => currentThirst == 0;
        public bool IsThirsty() => currentThirst < 70;
        public void setBar(GameObject barsContainer)
        {
            this.thirstBar = barsContainer.GetComponentInChildren<ThirstBar>();
            currentThirst = maxThirst;
            thirstBar.SetMaxThirst(maxThirst);
        }
        public void updateBar()
        {
            thirstBar.SetThirst(currentThirst);
        }
        public void Step()
        {
            if (animal.status == Status.DRINK)
                currentThirst = Mathf.Min(currentThirst + drinkAmount, maxThirst);
            else
                currentThirst--;

            if (IsDepleted())
            {
                OnThirstDepleted?.Invoke();
            }
            else if (IsFull())
            {
                OnThirstFull?.Invoke();
            }
            else if (IsCritical())
            {
                OnThirstCritical?.Invoke();
            }
        }
        public void StartDrinking()
        {
            animal.breakCounter = drinkDuration;
        }
        public void FinishDrinking()
        {
            return;
        }
    }
}
