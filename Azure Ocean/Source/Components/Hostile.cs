using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    public class Hostile : Component
    {
        public string name;
        public int visionRange = 5;
        public int territoryRange = 5;

        public float waited = 0f;

        public Hostile(string name)
        {
            this.name = name;
        }
    }
}
