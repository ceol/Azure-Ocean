using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Actions
{
    public abstract class Action
    {
        public Entity actor;
        public Components.Actor actorInfo;

        public Action(Entity actor)
        {
            this.actor = actor;
            actorInfo = actor.GetComponent<Components.Actor>();
        }

        public abstract ActionResult Perform();
    }

    public class Wait : Action
    {
        public Wait(Entity actor) : base(actor) { }

        public override ActionResult Perform()
        {
            return ActionResult.SUCCESS;
        }
    }

    public struct ActionResult
    {
        public bool successful;
        public Action alternative;

        public static ActionResult SUCCESS
        {
            get
            {
                return new ActionResult() { successful = true };
            }
        }

        public static ActionResult FAILURE
        {
            get
            {
                return new ActionResult() { successful = false };
            }
        }
    }
}
