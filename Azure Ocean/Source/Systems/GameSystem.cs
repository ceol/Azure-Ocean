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

        public List<Entity> entities;

        public GameSystem(Game game)
        {
            this.game = game;
        }

        public virtual void Run()
        {

        }
    }

    public class PlayerInputSystem : GameSystem
    {
        Type[] Components = { typeof(Player) };

        public PlayerInputSystem(Game game) : base(game) { }

        public override void Run()
        {
            entities = game.GetEntities(Components);
            Player playerComponent = entities.First().GetComponent<Player>();

        }
    }

    // Handles existence on the board
    public class PhysicsSystem : GameSystem
    {
        Type[] Components = { typeof(Position) };

        public PhysicsSystem(Game game) : base(game) { }

        public override void Run()
        {
            entities = game.GetEntities(Components);
        }
    }

    public class ActorSystem : GameSystem
    {
        Type[] Components = { typeof(Actor) };

        public ActorSystem(Game game) : base(game) { }

        public override void Run()
        {
            entities = game.GetEntities(Components);
            foreach (Entity entity in entities)
            {
                Debug.WriteLine(entity.name);
            }
        }
    }

    // Actually rendered to the screen instead of hidden
    public class RenderSystem : GameSystem
    {
        Type[] Components = { typeof(Render) };

        public RenderSystem(Game game) : base(game) { }

        public override void Run()
        {
            entities = game.GetEntities(Components);
        }
    }
}
