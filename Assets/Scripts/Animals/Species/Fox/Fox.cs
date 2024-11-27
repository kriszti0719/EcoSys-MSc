using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animals.Species.Fox    // ???
{
    public class Fox : Animal
    {
        protected override void Start()
        {
            base.Start();
        }
        public override int getTargetLayerToMate()
        {
            return LayerMask.GetMask("Fox"); ;
        }
        public override int getTargetLayerToEat()
        {
            return LayerMask.GetMask("Bunny"); ;
        }
        protected override void setTargetLayerToEat()
        {
            sensor.targetMask = LayerMask.GetMask("Bunny");
        }
        public override void setTargetLayerToMate()
        {
            sensor.targetMask = LayerMask.GetMask("Fox");
        }
    }
}
