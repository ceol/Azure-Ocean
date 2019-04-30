using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Components;

namespace AzureOcean.Actions
{
    public abstract class TargetedAction : GameAction
    {
        public Entity target;

        public TargetedAction(Actor actor, Entity target) : base(actor)
        {
            this.target = target;
        }
    }

    // Basic raw damage attack
    public class Attack : TargetedAction
    {
        public Attack(Actor actor, Entity target) : base(actor, target) { }

        public override ActionResult Perform()
        {
            return ActionResult.FAILURE;
        }
    }

    public class Heal : TargetedAction
    {
        public Heal(Actor actor, Entity target) : base(actor, target) { }

        public override ActionResult Perform()
        {
            return ActionResult.FAILURE;
        }
    }

    // An action that occurs passively each turn
    public class ApplyEffect : TargetedAction
    {
        GameAction effect;

        public ApplyEffect(Actor actor, Entity target, GameAction effect) : base(actor, target)
        {
            this.effect = effect;
        }

        public override ActionResult Perform()
        {
            return new ActionResult() { succeeded = false, alternative = effect };
        }
    }
}
