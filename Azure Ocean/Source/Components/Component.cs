using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AzureOcean.Components
{
    public class Component
    {
        public Entity entity;

        public virtual void ReceiveEvent(GameEvent gameEvent)
        {

        }

        public void SendEvent(string id)
        {
            GameEvent gameEvent = new GameEvent() { sender = entity, id = id };
            SendEvent(gameEvent);
        }

        public void SendEvent(string id, Dictionary<string, object> data)
        {
            GameEvent gameEvent = new GameEvent() { sender = entity, id = id, data = data };
            SendEvent(gameEvent);
        }

        public void SendEvent(GameEvent gameEvent)
        {
            entity.PropagateEvent(gameEvent);
        }
    }

    // Exists on the game's stage
    public class Transform : Component
    {
        public Point position;
    }

    public class Health : Component
    {
        int health;

        public Health(int startingHealth)
        {
            health = startingHealth;
        }

        public bool IsAlive
        {
            get { return health > 0; }
        }

        public override void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent.id == "TakeDamage")
            {
                int amount = (int)gameEvent.data["damage"];
                TakeDamage(amount);
                if (!IsAlive)
                    SendEvent("Died");
            }    
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health < 0) health = 0;
        }
    }

    public class Actor : Component
    {
        public Game game;

        int energy;
        int energyPerTurn = 1;

        Action nextAction;

        public Actor()
        {
            energy = 0;
        }

        public Actor(int startingEnergy)
        {
            energy = startingEnergy;
        }

        public Actor(Game game)
        {
            this.game = game;
        }

        public void GrantEnergy(int energy)
        {
            this.energy += energy;
        }

        public void SpendEnergy()
        {
            energy -= energyPerTurn;
        }

        public void SetAction(Action action)
        {
            nextAction = action;
        }

        public Action GetAction()
        {
            Action action = nextAction;
            nextAction = null;
            return action;
        }
    }

    public class Player : Component
    {

    }

    public class Hostile : Component
    {

    }

    public class Render : Component
    {
        public readonly string content;

        public Render(string content)
        {
            this.content = content;
        }
    }
}
