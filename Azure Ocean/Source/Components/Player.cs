using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    public class Player : Component
    {
        public string name;

        public Player()
        {
            name = "Player";
        }

        public Player(string name)
        {
            this.name = name;
        }
    }
}
