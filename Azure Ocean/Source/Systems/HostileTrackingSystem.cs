using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECS;
using AzureOcean.Components;

namespace AzureOcean.Systems
{
    class HostileTrackingSystem : ECS.System
    {
        struct Components
        {
            public Hostile hostile;
            public Transform transform;
        }

        struct TargetComponents
        {
            public Player player;
            public Transform transform;
        }

        float movementThreshold = 1f;
        int framesUntilMove = 60;

        public override void Run()
        {
            List<Entity<Components>> entities = entityManager.GetEntities<Components>();
            foreach (Entity<Components> entity in entities)
            {
                // Skip if no nearby targets
                List<Entity<TargetComponents>> targets = entityManager.GetEntities<TargetComponents>();
                if (targets.Count == 0)
                    continue;

                Entity<TargetComponents> target = targets.First();

                Hostile hostile = entity.components.hostile;
                hostile.waited += movementThreshold / framesUntilMove;

                if (hostile.waited >= movementThreshold)
                {
                    //entity.components.transform.velocity = Vector.up;
                    hostile.waited = 0f;
                }
            }
        }
    }
}
