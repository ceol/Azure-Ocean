using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Actions
{
    public class AttackAction : Action
    {
        Entity defender;

        public AttackAction(Entity actor, Entity defender) : base(actor) { this.defender = defender; }

        public override ActionResult Perform()
        {
            return ActionResult.SUCCESS;
        }
    }
}
