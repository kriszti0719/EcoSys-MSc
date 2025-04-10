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
        void Start()
        {
            animal = GetComponent<Animal>();
        }
<<<<<<< Updated upstream
        public void ToDie()
        {
            animal.prevStatus = animal.status;
            animal.status = Status.DIE;
        }
        public void Destroy()
        {
            if (animal.status == Status.DIE)
=======
        public void Destroy()
        {
            animal.GetComponentInParent<AnimalSpawner>().RegisterDeath(animal);
            foreach (GameObject g in animal.destructibles)
>>>>>>> Stashed changes
            {
                Destroy(g);
            }
<<<<<<< Updated upstream
=======
            Destroy(gameObject);
        }
        public void CatchPrey()
        {
            isCaptured = true;
>>>>>>> Stashed changes
        }
    }
}
