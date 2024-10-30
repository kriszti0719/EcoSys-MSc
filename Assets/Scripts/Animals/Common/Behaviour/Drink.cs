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
        public int drying;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
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
            currentThirst--;
        }
        public void StartDrinking()
        {
            animal.breakCounter = drinkDuration;
            animal.status = Status.DRINKING;
        }
        public void Drinking()
        {
            if (currentThirst + drinkAmount < maxThirst)
                currentThirst += drinkAmount;
            else
            {
                currentThirst = maxThirst;
            }
        }
        public void FinishDrinking()
        {
            return;
        }
        public bool isDrying()
        {
            return currentThirst <= drying;
        }
        public bool isFull()
        {
            return currentThirst == maxThirst;
        }
        public bool isThirsty()
        {
            return currentThirst < 70;
        }
        public bool isDriedToDeath()
        {
            return currentThirst == 0;
        }
    }
}
