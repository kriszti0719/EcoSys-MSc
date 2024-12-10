using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Datatypes
{
    internal class GenerateAnimal
    {
        public Animal mother;
        public MateTraits father;
        public Species species;
        public float sizeMin;
        public float sizeMax;

        public GenerateAnimal(Animal _mother, MateTraits _father, Species _species, float _sizeMin, float _sizeMax)
        {
            this.mother = _mother;
            this.father = _father;
            this.species = _species;
            this.sizeMin = _sizeMin;
            this.sizeMax = _sizeMax;
        }

        public GenerateAnimal(Animal _mother, MateTraits _father, Species _species)
        {
            this.mother = _mother;
            this.father = _father;
            this.species = _species;

            if (_father != null && _mother != null)
            {
                if(_father.size < _mother.aging.size)
                {
                    this.sizeMin = _father.size;
                    this.sizeMax = _mother.aging.size;
                }
                else
                {
                    this.sizeMin = _mother.aging.size;
                    this.sizeMax = _father.size;
                }
            }
            
        }
    }
}
