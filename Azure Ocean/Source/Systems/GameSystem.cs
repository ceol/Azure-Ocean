using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
    public abstract class GameSystem
    {
        public GameState game;

        public List<Entity> entities;

        public void Bind(GameState game)
        {
            this.game = game;
        }

        public virtual void Run()
        {

        }
    }

    public class PlayerInputSystem : GameSystem
    {
        struct Components
        {
            Actor actor;
            Player player;
        }

        KeyboardState previousState;

        public override void Run()
        {
            entities = game.GetEntities<Components>();
            Entity player = entities.First();
            Actor actor = player.GetComponent<Actor>();
            if (actor == null)
            {
                return;
            }

            if (previousState == null)
                previousState = Keyboard.GetState();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up) && !previousState.IsKeyDown(Keys.Up))
                actor.SetAction(new Walk(actor, Vector.up));
            if (state.IsKeyDown(Keys.Down) && !previousState.IsKeyDown(Keys.Down))
                actor.SetAction(new Walk(actor, Vector.down));
            if (state.IsKeyDown(Keys.Left) && !previousState.IsKeyDown(Keys.Left))
                actor.SetAction(new Walk(actor, Vector.left));
            if (state.IsKeyDown(Keys.Right) && !previousState.IsKeyDown(Keys.Right))
                actor.SetAction(new Walk(actor, Vector.right));

            previousState = state;
        }
    }

    // Implements logic for hostile actors
    // This sets their next action based on any available targets
    public class HostileSystem : GameSystem
    {
        struct Components
        {
            Actor actor;
            Hostile hostile;
        }

        Dictionary<Actor, Entity> targets = new Dictionary<Actor, Entity>();

        public override void Run()
        {
            entities = game.GetEntities<Components>();
            foreach (Entity entity in entities)
            {
                Actor actor = entity.GetComponent<Actor>();
                actor.SetAction(new Wait(actor));

                // See if it has a target or check for new target
                Entity target = GetTarget(actor);
                if (target == null)
                    continue;
                
                // Pathfind to target


                // Move to next available tile in path
            }
        }

        public Entity GetTarget(Actor actor)
        {
            Entity target = null;

            if (targets.ContainsKey(actor))
            {
                target = targets[actor];
                if (target == null)
                    targets.Remove(actor); // is this necessary?
                return target;
            }

            return target;
        }
    }

    public class ProjectileSystem : GameSystem
    {
        public override void Run()
        {
            
        }
    }

    // Handles existence on the board
    public class PhysicsSystem : GameSystem
    {
        struct Components
        {
            Transform transform;
        }

        public override void Run()
        {
            entities = game.GetEntities<Components>();
        }
    }

    public class TurnSystem : GameSystem
    {
        struct Components
        {
            Actor actor;
        }

        LinkedList<Actor> turnOrder = new LinkedList<Actor>();

        public override void Run()
        {
            if (turnOrder.Count == 0)
            {
                foreach (Entity entity in game.GetEntities<Components>())
                {
                    turnOrder.AddLast(entity.GetComponent<Actor>());
                }
            }

            // Process next actor
            LinkedListNode<Actor> actorNode = turnOrder.First;
            if (actorNode == null || actorNode.Value == null)
            {
                if (actorNode != null)
                    turnOrder.RemoveFirst();
                return;
            }
            Actor actor = actorNode.Value;

            actor.GrantEnergy();
            while (actor.CanAct)
            {
                GameAction action = actor.GetAction();
                if (action == null)
                {
                    actor.SpendEnergy();
                    return;
                }

                ActionResult result = action.Perform();
                while (result.alternative != null)
                {
                    action = result.alternative;
                    result = action.Perform();
                }

                actor.SpendEnergy();
            }

            turnOrder.AddLast(actor);
            turnOrder.RemoveFirst();
        }
    }

    // Actually rendered to the screen instead of hidden
    public class RenderSystem : GameSystem
    {
        struct Components
        {
            Transform transform;
            Render render;
        }

        public override void Run()
        {
            entities = game.GetEntities<Components>();
        }
    }
}
