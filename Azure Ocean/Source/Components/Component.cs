using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean.Components
{
    public class Component
    {
        
    }

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

    public class Actor : Component
    {
        int energy = 0;
        int energyPerTurn = 1;
        
        public bool CanAct
        {
            get { return energy > 0; }
        }
    }

    public class Player : Component
    {

    }

    public class Hostile : Component
    {
        public int visionRange = 5;
        public int territoryRange = 5;
    }

    public class Render : Component
    {
        public readonly string content;

        public Render(string content)
        {
            this.content = content;
        }
    }
}
