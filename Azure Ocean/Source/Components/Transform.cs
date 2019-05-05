using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    // Exists on the game's stage
    public class Transform : Component
    {
        public Vector position;
        public Vector velocity;

        public Transform()
        {
            position = new Vector(0, 0);
        }

        public Transform(Vector position)
        {
            this.position = position;
        }
    }
}
