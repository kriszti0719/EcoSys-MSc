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
        int caught = 0;
        int other = 0;

        void Start()
        {
            animal = GetComponent<Animal>();
        }
        public void ToDie()
        {
            if(animal.status == Status.CAUGHT)
            {
                Destroy();
                return;
            }
            animal.prevStatus = animal.status;
            animal.status = Status.DIE;
        }
        public void Destroy()
        {
            if (animal.status == Status.DIE || animal.status == Status.CAUGHT)
            {
                 if (caught+other > 0)
                {
                    Debug.Log("Already Dead Caught Other " +caught+ "Other " + other);
                    return;
                }
                if (animal.status == Status.DIE)
                    other++;
                else if (animal.status == Status.CAUGHT)
                    caught++;

                animal.GetComponentInParent<AnimalSpawner>().RegisterDeath(animal);
                foreach (GameObject g in animal.destructibles)
                {
                    Destroy(g);
                }
                Destroy(gameObject);
            }
        }
    }
}
