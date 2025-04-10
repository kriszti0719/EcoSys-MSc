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

        public event Action OnRestFull;
        public event Action OnRestDepleted;

        void Start()
        {
            restAmount = Mathf.RoundToInt(maxStamina * 0.05f);
            animal = GetComponent<Animal>();
        }
        private bool IsRested() => currentStamina == maxStamina;
        private bool IsDepleted() => currentStamina == 0;
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
            if (animal.status == Status.REST || animal.status == Status.EAT || animal.status == Status.DRINK)
                currentStamina = Mathf.Min(currentStamina + restAmount, maxStamina);
            else
                currentStamina--;
            if(IsRested())
            {
                OnRestDepleted?.Invoke();
            }
            else if (IsDepleted())
            {
                OnRestDepleted?.Invoke();
            }
        }
        public bool ChanceToRest()
        {
            if (currentStamina > 30) return false;

            float restProbability = Mathf.Clamp01(1f - (currentStamina / 30f)); // Minél alacsonyabb a stamina, annál nagyobb az esély

            if (UnityEngine.Random.value < restProbability)
            {
                int restAmount = (currentStamina == 0) ? 100 : UnityEngine.Random.value < 0.5f ? 50 : 20;
                ToRest(restAmount);
                return true;
            }
            return false;
        }
        public void ToRest(int time)
        {
            if (animal.status != Status.DIE)
            {
<<<<<<< Updated upstream
                animal.prevStatus = animal.status;
                animal.status = Status.RESTING;
=======
>>>>>>> Stashed changes
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
    }
}
