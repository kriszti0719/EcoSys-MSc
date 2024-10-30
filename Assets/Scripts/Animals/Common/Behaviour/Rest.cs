using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Rest : MonoBehaviour
    {
        private Animal animal;
        [SerializeField]
        private int restAmount;             // X mp alatt mennyit pihen
        public StaminaBar staminaBar;      //StaminaBar
        private int maxStamina = 100;
        [SerializeField]
        private int currentStamina;

        void Start()
        {
            restAmount = Mathf.RoundToInt(maxStamina * 0.05f);
            animal = GetComponent<Animal>();
        }
        public void setBar(GameObject barsContainer) {

            this.staminaBar = barsContainer.GetComponentInChildren<StaminaBar>();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
        }
        public void updateBar()
        {
            staminaBar.SetStamina(currentStamina);
        }
        public void Step()
        {
            currentStamina--;
        }       
        public void ChanceToRest()
        {
            if (currentStamina <= 30 && currentStamina >= 10)
            {
                // According to sum other circumstances it can decide
                //  - to move on OR
                //  - to sleep
                //  - to stop for a normal break
                //  - to stop for a little break

                int randomValue = UnityEngine.Random.Range(0, 4);
                switch (randomValue)
                {
                    case 0:
                        ToRest(20);
                        break;
                    case 1:
                        ToRest(50);
                        break;
                    default:
                        break;
                }
            }
            else if (currentStamina <= 10 && currentStamina >= 0)
            {
                // According to sum other circumstances it can decide
                //  - to move on OR
                //  - to sleep
                //  - to stop for a normal break
                //  - to stop for a little break

                int randomValue = UnityEngine.Random.Range(0, 3);
                switch (randomValue)
                {
                    case 0:
                        ToRest(20);
                        break;
                    case 1:
                        ToRest(50);
                        break;
                    default:
                        break;
                }
            }
            else if (currentStamina <= 0)
            {
                // According to sum other circumstances it can decide
                //  - to sleep
                //  - to stop for a normal break
                int randomValue = UnityEngine.Random.Range(0, 2);
                if (randomValue == 0)
                {
                    ToRest(100);
                }
                else if (randomValue == 1)
                {
                    ToRest(50);
                }
            }
        }
        public void ToRest(int time)
        {
            if (animal.status != Status.DIE)
            {
                animal.prevStatus = animal.status;
                animal.status = Status.RESTING;
                if (currentStamina + time < maxStamina)
                {
                    animal.breakCounter = time;
                }
                else
                {
                    animal.breakCounter = maxStamina - currentStamina;
                }
            }
        }
        public void Resting()
        {
            if (currentStamina + restAmount < maxStamina)
                currentStamina += restAmount;
            else
            {
                currentStamina = maxStamina;
            }
        }
        public bool isRested()
        {
            return currentStamina >= maxStamina;
        }
        public bool isFainted()
        {
            return currentStamina <= 0;
        }
    }
}
