using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureOcean.Components;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean.Actions
{
    public abstract class GameAction 
    {
        public Actor actor;

        public GameAction (Actor actor)
        {
            this.actor = actor;
        }

        public abstract ActionResult Perform();
    }

    public class Wait : GameAction
    {
        public Wait(Actor actor) : base(actor) { }

        public override ActionResult Perform()
        {
            return ActionResult.SUCCESS;
        }
    }

    public class Walk : GameAction 
    {
        Vector direction;

        public Walk(Actor actor, Vector direction) : base(actor)
        {
            this.direction = direction;
        }

        public override ActionResult Perform()
        {
            Vector position = actor.GetPosition();
            if (position == null)
            {
                return ActionResult.FAILURE;
            }
            
            Vector destination = position + direction;
            Stage stage = actor.game.CurrentStage;

            Entity occupant = stage.GetOccupant(destination);
            if (occupant != null)
            {
                return new ActionResult() { succeeded = false, alternative = new AttackAction(actor, occupant) };
            }
            
            if (!stage.IsTraversable(destination))
            {
                return ActionResult.FAILURE;
            }
            
            actor.SetPosition(destination);
            stage.MoveTo(actor.entity, destination);
            return ActionResult.SUCCESS;
        }
    }

    public class WalkTowards : GameAction
    {
        public Vector destination;

        public WalkTowards(Actor actor, Vector destination) : base(actor)
        {
            this.destination = destination;
        }

        public override ActionResult Perform()
        {
            Debug.WriteLine(actor.entity.name + " walks towards " + destination);

            Vector start = actor.GetPosition();
            HostilePathfinder pathfinder = new HostilePathfinder(actor.game.CurrentStage);
            List<Vector> steps = pathfinder.GetSteps(start, destination);

            // Move to next available tile in path
            if (steps.Count > 0)
            {
                Vector nextStep = steps.First();
                Vector stepDirection = nextStep - start;
                return new ActionResult() { succeeded = false, alternative = new Walk(actor, stepDirection) };
            }

            return ActionResult.FAILURE;
        }
    }

    public class AttackAction : GameAction
    {
        public Entity defender;

        public AttackAction(Actor actor, Entity defender) : base(actor)
        {
            this.defender = defender;
        }

        public override ActionResult Perform()
        {
            Debug.WriteLine(actor.entity.name + " attacks " + defender.name);
            return ActionResult.SUCCESS;
        }
    }

    public class HostileAction : GameAction
    {
        public Entity target;

        public HostileAction(Actor actor, Entity target) : base(actor)
        {
            this.target = target;
        }

        public override ActionResult Perform()
        {
            if (target == null)
                return ActionResult.FAILURE;

            // Pathfind to target
            Vector start = actor.GetPosition();
            Vector destination = target.GetComponent<Transform>().position;

            HostilePathfinder pathfinder = new HostilePathfinder(actor.game.CurrentStage);
            List<Vector> steps = pathfinder.GetSteps(start, destination);

            // Move to next available tile in path
            if (steps.Count > 0)
            {
                Vector nextStep = steps.First();
                Vector stepDirection = nextStep - start;
                return new ActionResult() { succeeded = false, alternative = new Walk(actor, stepDirection) };
            }

            return ActionResult.FAILURE;
        }
    }

    public struct ActionResult
    {
        public bool succeeded;
        public GameAction alternative;

        public static ActionResult SUCCESS
        {
            get { return new ActionResult() { succeeded = true }; }
        }

        public static ActionResult FAILURE
        {
            get { return new ActionResult() { succeeded = false }; }
        }
    }
}
