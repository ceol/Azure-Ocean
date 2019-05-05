using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECS;
using AzureOcean.Components;

namespace AzureOcean.Systems
{
    class MovementSystem : ECS.System
    {
        struct Components
        {
            public Transform transform;
        }

        public override void Run()
        {
            List<Entity<Components>> entities = entityManager.GetEntities<Components>();
            foreach (Entity<Components> entity in entities)
            {
                Transform transform = entity.components.transform;
                if (transform.velocity != default(Vector))
                {
                    transform.position += transform.velocity;
                    transform.velocity = default(Vector);
                }
            }
        }
    }
}
