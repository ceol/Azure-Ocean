using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = System.Random;

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
        int framesUntilMove = 100;

        Random random = new Random();

        public override void Run()
        {
            List<Entity<Components>> entities = entityManager.GetEntities<Components>();
            foreach (Entity<Components> entity in entities)
            {
                if (!HasTargets(entity))
                    continue;
                Entity<TargetComponents> target = GetClosestTarget();

                Hostile hostile = entity.components.hostile;
                hostile.waited += movementThreshold / framesUntilMove;

                if (hostile.waited >= movementThreshold)
                {
                    entity.components.transform.velocity = Vector.cardinals[random.Next(4)];
                    hostile.waited = 0f;
                }
            }
        }

        bool HasTargets(Entity<Components> entity)
        {
            return true;
        }

        Entity<TargetComponents> GetClosestTarget()
        {
            List<Entity<TargetComponents>> targets = entityManager.GetEntities<TargetComponents>();
            return targets.First();
        }
    }
}
