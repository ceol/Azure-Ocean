using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

using ECS;
using AzureOcean.Components;

namespace AzureOcean.Systems
{
    class PlayerInputSystem : ECS.System
    {
        KeyboardState previousState;

        struct Components
        {
            public Player player;
            public Transform transform;
        }

        public override void Run()
        {
            List<Entity<Components>> entities = entityManager.GetEntities<Components>();
            Entity<Components> playerEntity = entities.First();

            if (previousState == null)
                previousState = Keyboard.GetState();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up) && !previousState.IsKeyDown(Keys.Up))
                playerEntity.components.transform.velocity = Vector.up;
            if (state.IsKeyDown(Keys.Down) && !previousState.IsKeyDown(Keys.Down))
                playerEntity.components.transform.velocity = Vector.down;
            if (state.IsKeyDown(Keys.Left) && !previousState.IsKeyDown(Keys.Left))
                playerEntity.components.transform.velocity = Vector.left;
            if (state.IsKeyDown(Keys.Right) && !previousState.IsKeyDown(Keys.Right))
                playerEntity.components.transform.velocity = Vector.right;

            previousState = state;
        }
    }
}
