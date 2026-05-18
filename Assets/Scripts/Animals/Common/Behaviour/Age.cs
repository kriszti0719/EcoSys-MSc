using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Age : MonoBehaviour
    {
        private Animal animal;
        private float aging = 0.5f;
        public float secCnt;
        public float currentAge;
        public float lifeSpan;
        public float size;

        public event Action OnAgeLimitReached;
        void Start()
        {
            animal = GetComponent<Animal>();
            secCnt = 0;
        }
        private bool AgeLimitReached() => currentAge >= lifeSpan;
        public void setAging(float _age, float _size, float _lifeSpan)
        {
            this.currentAge = _age;
            this.size = _size;
            this.lifeSpan = _lifeSpan;
        }
        public void Aging()
        {
            if (currentAge < 1)
            {
                Grow();
                currentAge += aging / 100f;
                if (currentAge >= 1)
                {
                    animal.reproduction.Mature();
                    animal.mating.enableMating = true;
                }
            }

            currentAge += aging;
            if (AgeLimitReached())
            {
                OnAgeLimitReached?.Invoke();
            }
        }
        private void Grow()
        {
            animal.barsContainer.transform.SetParent(null);
            if (currentAge < 0.4)
                animal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            else
                animal.transform.localScale = new Vector3(size * currentAge, size * currentAge, size * currentAge);
            animal.barsContainer.transform.SetParent(animal.transform);
        }
    }
}
