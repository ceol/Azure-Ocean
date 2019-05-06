using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Components;

namespace AzureOcean.Systems
{
    // Handles starting/stopping battles
    // Creates/destroys the entities that BattleSystem will use
    public class ManageBattleSystem : ECS.System
    {
        public override void Run()
        {
            // If we should be in a battle,

            // Else if we should end a currently running battle
        }
    }

    // Progress through turns until one party is empty
    public class BattleSystem : ECS.System
    {
        public override void Run()
        {

        }
    }
}
