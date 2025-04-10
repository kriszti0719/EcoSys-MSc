using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animals.Common.Behaviour
{
    public class Die : MonoBehaviour
    {
        private Animal animal;
        public bool isCaptured = false;

        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void Destroy()
        {
            animal.GetComponentInParent<AnimalSpawner>().RegisterDeath(animal);
            foreach (GameObject g in animal.destructibles)
            {
                Destroy(g);
            }
            Destroy(gameObject);
        }
        public void CatchPrey()
        {
            isCaptured = true;
        }
    }
}
