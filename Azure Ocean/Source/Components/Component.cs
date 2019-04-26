using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Debug = System.Diagnostics.Debug;

using AzureOcean.Actions;

namespace AzureOcean.Components
{
    public class Component
    {
        public Entity entity;

        public void Bind(Entity entity)
        {
            this.entity = entity;
        }

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
        public Vector position;

        public Transform()
        {
            position = new Vector(0, 0);
        }

        public Transform(Vector position)
        {
            this.position = position;
        }
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
        public GameState game;

        int energy = 0;
        int energyPerTurn = 1;

        GameAction nextAction;

        public Actor(GameState game)
        {
            this.game = game;
        }
        
        public bool CanAct
        {
            get { return energy > 0; }
        }

        public void GrantEnergy()
        {
            energy += energyPerTurn;
        }

        public void SpendEnergy()
        {
            energy -= energyPerTurn;
        }

        public void SetAction(GameAction action)
        {
            nextAction = action;
        }

        public GameAction GetAction()
        {
            GameAction action = nextAction;
            nextAction = null;
            return action;
        }

        public Vector GetPosition()
        {
            Transform transform = entity.GetComponent<Transform>();
            return transform.position;
        }

        public void SetPosition(Vector coordinate)
        {
            Transform transform = entity.GetComponent<Transform>();
            if (transform == null)
                return;

            transform.position = coordinate;
        }
    }

    public class Player : Component
    {

    }

    public class Hostile : Component
    {
        public int visionRange = 5;
        public int territoryRange = 5;
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
