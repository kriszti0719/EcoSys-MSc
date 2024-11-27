using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Datatypes
{
    internal class GenerateAnimal
    {
        public GameObject prefab;
        public Animal mother;
        public Animal father;
        public Species species;
        public float sizeMin;
        public float sizeMax;

        public GenerateAnimal(GameObject _prefab, Animal _mother, Animal _father, Species _species, float _sizeMin, float _sizeMax)
        {
            this.prefab = _prefab;
            this.mother = _mother;
            this.father = _father;
            this.species = _species;
            this.sizeMin = _sizeMin;
            this.sizeMax = _sizeMax;
        }
    }
}
