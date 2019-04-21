using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;
using AzureOcean.Components;

namespace AzureOcean
{
    public abstract class GameSystem
    {
        public Game game;

        public GameSystem(Game game)
        {
            this.game = game;
        }

        public virtual void Run()
        {

        }
    }

    // Handles existence on the board
    public class PhysicsSystem : GameSystem
    {
        Type[] Components = { typeof(Position) };

        public PhysicsSystem(Game game) : base(game) { }

        public override void Run()
        {
            List<Entity> entities = game.GetEntities(Components);
        }
    }

    public class ActorSystem : GameSystem
    {
        Type[] Components = { typeof(Energy) };

        public ActorSystem(Game game) : base(game) { }

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
        public RenderSystem(Game game) : base(game) { }

        public override void Run()
        {
            
        }
    }
}
