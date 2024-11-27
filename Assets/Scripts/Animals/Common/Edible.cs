using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal interface Edible
    {
        int getEatDuration();
        void setStatusCaught();
        int getNutrition();
        void Eaten();
    }
}
