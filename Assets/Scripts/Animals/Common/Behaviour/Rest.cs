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
        [HideInInspector] public StaminaBar staminaBar;    
        public int maxStamina = 100;
        public int currentStamina;
        [SerializeField]
        private int restAmount = 5;
        public int breakCounter = 0;
        public event Action OnRestDepleted;
        public event Action OnBreakEnded;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        private bool IsRested() => currentStamina == maxStamina;
        private bool IsDepleted() => currentStamina == 0;
        private bool IsBreakEnded() => breakCounter == 0;
        public void setBar(GameObject barsContainer, bool randomize = false) {

            staminaBar = barsContainer.GetComponentInChildren<StaminaBar>();
            currentStamina = randomize ? UnityEngine.Random.Range(30, maxStamina) : maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
        }
        public void updateBar()
        {
            staminaBar.SetStamina(currentStamina);
        }
        public void Step()
        {
            if (animal.status == Status.REST)
            {
                currentStamina = Mathf.Min(currentStamina + restAmount, maxStamina);
                breakCounter = System.Math.Max(0, breakCounter - 1);
                if (IsRested() || (IsBreakEnded()))
                {
                    OnBreakEnded?.Invoke();
                }
            }
            else
            {
                currentStamina--;
                if (IsDepleted())
                {
                    OnRestDepleted?.Invoke();
                }
            }
        }
        public bool ChanceToRest()
        {
            if (currentStamina > 30) return false;

            float restProbability = Mathf.Clamp01(1f - (currentStamina / 30f));

            if (UnityEngine.Random.value < restProbability)
            {
                int time = (currentStamina == 0) ? 100 : UnityEngine.Random.value < 0.5f ? 50 : 20;
                ToRest(time);
                return true;
            }
            return false;
        }
        public void ToRest(int time)
        {
            if (restAmount <= 0) { breakCounter = 0; return; }
            int staminaToRestore = Mathf.Min(time, maxStamina - currentStamina);
            breakCounter = Mathf.Max(0, Mathf.CeilToInt((float)staminaToRestore / restAmount));
        }
    }
}
