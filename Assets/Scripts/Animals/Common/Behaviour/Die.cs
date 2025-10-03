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
            DebugLogger.RegisterDeath(step: animal.GetComponentInParent<LoggerSettings>().getStep(), animal: animal);
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
