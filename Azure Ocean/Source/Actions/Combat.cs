using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Components;

namespace AzureOcean.Actions
{
    public class CombatAction
    {
        public List<Entity> targets = new List<Entity>();
        public List<ActionComponent> components = new List<ActionComponent>();
    }

    public class ActionComponent
    {

    }
}
