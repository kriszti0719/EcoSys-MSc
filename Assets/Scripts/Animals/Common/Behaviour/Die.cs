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
        public void ToDie()
        {
            if(animal.status == Status.CAUGHT)
            {
                Destroy();
            }
            animal.prevStatus = animal.status;
            animal.status = Status.DIE;
        }
        public void Destroy()
        {
            if (animal.status == Status.DIE || animal.status == Status.CAUGHT)
            {
                animal.GetComponentInParent<AnimalSpawner>().RegisterDeath(animal);
                foreach (GameObject g in animal.destructibles)
                {
                    Destroy(g);
                }
                Destroy(gameObject);
            }
        }

        public void BeingEaten()
        {
            animal.status = Status.CAUGHT;
            Destroy();
        }
        public void CatchPrey()
        {
            isCaptured = true;
        }
    }
}
