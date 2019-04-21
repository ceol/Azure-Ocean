using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
            Entity player = entities.First();
            Transform transform = player.GetComponent<Transform>();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                transform.position += new Point(0, -1);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                transform.position += new Point(0, 1);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                transform.position += new Point(-1, 0);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                transform.position += new Point(1, 0);
        }
    }

    // Handles existence on the board
    public class PhysicsSystem : GameSystem
    {
        Type[] Components = { typeof(Transform) };

        public PhysicsSystem(Game game) : base(game) { }

        public override void Run()
        {
            entities = game.GetEntities(Components);
        }
    }

    public class ActorSystem : GameSystem
    {
        Type[] Components = { typeof(Actor) };

        LinkedList<Entity> actors;

        public ActorSystem(Game game) : base(game) { }

        public override void Run()
        {
            
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
