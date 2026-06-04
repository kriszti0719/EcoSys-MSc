using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Age : MonoBehaviour
    {
        private Animal animal;
        private float aging = 0.5f;
        public float adultSize;
        public float lifeSpan;
        public float currentAge;

        public event Action OnAgeLimitReached;
        void Start()
        {
            animal = GetComponent<Animal>();
        }
        private bool AgeLimitReached() => currentAge >= lifeSpan;
        public void setAging(float _age, float _size, float _lifeSpan)
        {
            currentAge = _age;
            adultSize = _size;
            lifeSpan = _lifeSpan;
        }
        public void Aging()
        {
            if (currentAge < 1)
            {
                Grow();
                currentAge += aging / 100f;
            }

            currentAge += aging;
            
            if (AgeLimitReached())
            {
                OnAgeLimitReached?.Invoke();
            }
        }
        private void Grow()
        {
            if (animal.barsContainer == null) return;
            animal.barsContainer.transform.SetParent(null);
            if (currentAge < 0.4)
                animal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            else
                animal.transform.localScale = new Vector3(adultSize * currentAge, adultSize * currentAge, adultSize * currentAge);
            animal.barsContainer.transform.SetParent(animal.transform);
        }
    }
}
