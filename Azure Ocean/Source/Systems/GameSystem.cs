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
        public GameEngine game;

        public List<Entity> entities;

        public void Bind(GameEngine game)
        {
            this.game = game;
        }

        public virtual void Run()
        {

        }
    }

    public class PlayerInputSystem : GameSystem
    {
        Type[] Components = { typeof(Player) };

        KeyboardState previousState;

        public override void Run()
        {
            entities = game.GetEntities(Components);
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

    public class HostileSystem : GameSystem
    {
        Type[] Components = { typeof(Hostile), typeof(Actor) };

        public override void Run()
        {
            entities = game.GetEntities(Components);
            foreach (Entity entity in entities)
            {
                Actor actor = entity.GetComponent<Actor>();
                actor.SetAction(new Walk(actor, Vector.left));
            }
        }
    }

    // Handles existence on the board
    public class PhysicsSystem : GameSystem
    {
        Type[] Components = { typeof(Transform) };

        public override void Run()
        {
            entities = game.GetEntities(Components);
        }
    }

    public class TurnSystem : GameSystem
    {
        Type[] Components = { typeof(Actor) };

        LinkedList<Actor> turnOrder = new LinkedList<Actor>();

        public override void Run()
        {
            if (turnOrder.Count == 0)
            {
                foreach (Entity entity in game.GetEntities(Components))
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
        Type[] Components = { typeof(Render) };

        public override void Run()
        {
            entities = game.GetEntities(Components);
        }
    }
}
