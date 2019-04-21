using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean
{
    public class GameSystem
    {
        public Game game;

        public virtual void Run()
        {

        }
    }

    public class ActorSystem : GameSystem
    {
        string[] Components = { "Actor" };

        public override void Run()
        {
            List<Entity> entities = game.GetEntities(Components);
            foreach (Entity entity in entities)
            {
                
            }
        }
    }

    public class RenderSystem : GameSystem
    {
        public override void Run()
        {
            
        }
    }
}
