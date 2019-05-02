using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Actions;
using AzureOcean.Components;

namespace AzureOcean.Systems
{
    public class CombatSystem : GameSystem
    {
        LinkedList<Actor> turnOrder = new LinkedList<Actor>();

        struct Components
        {
            public Actor actor;
        }

        public override void Run()
        {
            if (!game.inCombat) return;

            if (turnOrder.Count == 0)
            {
                foreach (Entity entity in game.GetEntities<Components>())
                {
                    turnOrder.AddLast(entity.GetComponent<Actor>());
                }
            }

            // Get the next actor
            LinkedListNode<Actor> actorNode = turnOrder.First;
            if (actorNode == null || actorNode.Value == null)
            {
                // Somehow an empty node got in here, so remove it
                if (actorNode != null)
                    turnOrder.RemoveFirst();
                return;
            }
            Actor actor = actorNode.Value;

            // Begin their turn
            actor.GrantEnergy();
            while (actor.CanAct)
            {
                GameAction action = actor.GetAction();
                if (action == null)
                {
                    // If they're not ready, reset their turn
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

            // Need to check if we should still be in battle
        }
    }
}
