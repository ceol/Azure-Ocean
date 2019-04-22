using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Actions
{
    public class Walk : Action
    {
        public readonly Point direction;

        public Walk(Entity actor, Point direction) : base(actor) { this.direction = direction; }

        public override ActionResult Perform()
        {
            Components.Transform transform = actor.GetComponent<Components.Transform>();

            Stage stage = actorInfo.game.CurrentStage;
            Point destination = transform.position + direction;

            // check occupancy
            Entity occupant = stage.GetOccupant(destination);
            if (occupant != null)
            {
                AttackAction attack = new AttackAction(actor, occupant);
                return new ActionResult() { successful = false, alternative = attack };
            }

            if (!stage.IsTraversable(destination.X, destination.Y))
                return ActionResult.FAILURE;

            transform.position = destination;
            return ActionResult.SUCCESS;
        }
    }
}
