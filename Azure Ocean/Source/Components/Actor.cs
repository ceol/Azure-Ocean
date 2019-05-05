using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    public class Actor : Component
    {
        public int energy = 0;
        public int energyPerTurn = 1;

        public bool CanAct
        {
            get { return energy > 0; }
        }
    }
}
