using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    public class Health : Component
    {
        int health;

        public Health(int startingHealth)
        {
            health = startingHealth;
        }

        public bool IsAlive
        {
            get { return health > 0; }
        }
    }
}
