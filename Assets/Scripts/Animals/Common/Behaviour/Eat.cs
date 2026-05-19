using System;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Eat : MonoBehaviour
    {
        private Animal animal;
        [HideInInspector] public HungerBar hungerBar;
        public int maxHunger = 100;
        public int critical;
        public int currentHunger;

        public event Action OnHungerCritical;
        public event Action OnHungerDepleted;

        void Start() { animal = GetComponent<Animal>(); }
        private bool IsCritical() => currentHunger <= critical;
        private bool IsDepleted() => currentHunger == 0;
        public bool IsHungry() => currentHunger < 70;
        public void setBar(GameObject barsContainer, bool randomize = false)
        {
            this.hungerBar = barsContainer.GetComponentInChildren<HungerBar>();
            currentHunger = randomize ? UnityEngine.Random.Range(critical, maxHunger) : maxHunger;
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

            if (IsDepleted())
            {
                OnHungerDepleted?.Invoke();
            }
            else if (IsCritical())
            {
                OnHungerCritical?.Invoke();
            }
        }
        public void Eating()
        {
            if (animal.targetRef != null && animal.targetRef.TryGetComponent<IEdible>(out var edibleFood))
            {
                currentHunger = Mathf.Min(currentHunger + edibleFood.getNutrition(), maxHunger);
                edibleFood.Consumed();
                
                animal.targetRef = null;
                animal.sensor.targetMask = LayerMask.GetMask("None");
            }
            else
            {
                (animal.status, animal.prevStatus) = (animal.prevStatus, Status.WANDER);
                animal.targetRef = null;
            }
        }
    }
}
