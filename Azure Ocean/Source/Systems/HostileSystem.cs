using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

using AzureOcean.Actions;
using AzureOcean.Components;

namespace AzureOcean
{
    // Implements logic for hostile actors
    // This sets their next action based on any available targets
    public class HostileSystem : GameSystem
    {
        struct Components
        {
            public Actor actor;
            public Hostile hostile;
            public Transform transform;
        }

        Dictionary<Actor, Vector> spawnPoints = new Dictionary<Actor, Vector>();

        public override void Run()
        {
            entities = game.GetEntities<Components>();
            foreach (Entity entity in entities)
            {
                Actor actor = entity.GetComponent<Actor>();
                actor.SetAction(new Wait(actor)); // default

                Transform transform = entity.GetComponent<Transform>();

                if (!spawnPoints.ContainsKey(actor))
                    spawnPoints[actor] = transform.position;
                Vector spawn = spawnPoints[actor];

                Hostile hostile = entity.GetComponent<Hostile>();

                if (!WithinRange(transform.position, spawn, hostile.territoryRange))
                {
                    actor.SetAction(new WalkTowards(actor, spawn));
                    continue;
                }

                Entity target = GetTarget(actor);
                if (target != null)
                {
                    Transform targetTransform = target.GetComponent<Transform>();
                    if (WithinRange(targetTransform.position, spawn, hostile.territoryRange))
                    {
                        actor.SetAction(new HostileAction(actor, target));
                        continue;
                    }
                }

                if (transform.position != spawn)
                    actor.SetAction(new WalkTowards(actor, spawn));
            }
        }

        public Entity GetTarget(Actor actor)
        {
            Entity target = null;

            List<Vector> visible = GetVisibleVectors(actor);
            foreach (Vector nearPosition in visible)
            {
                target = game.CurrentStage.GetOccupant(nearPosition);
                if (target != null && target != actor.entity)
                    return target;
            }

            return target;
        }

        public List<Vector> GetVisibleVectors(Actor actor)
        {
            Hostile hostile = actor.entity.GetComponent<Hostile>();
            Transform transform = actor.entity.GetComponent<Transform>();
            Vector position = transform.position;
            List<Vector> positions = new List<Vector>();

            // https://stackoverflow.com/a/1237519
            int radius = hostile.visionRange;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                        positions.Add(new Vector(position.x + x, position.y + y));
                }
            }

            return positions;
        }

        public bool WithinRange(Vector coordinate, Vector center, int radius)
        {
            int lowerX = center.x - radius;
            int upperX = center.x + radius;
            int lowerY = center.y - radius;
            int upperY = center.y + radius;

            return coordinate.x >= lowerX && coordinate.x <= upperX && coordinate.y >= lowerY && coordinate.y <= upperY;
        }
    }
}
