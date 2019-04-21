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

        public string GetName()
        {
            return GetType().ToString().Replace("AzureOcean.Components.", "");
        }
    }

    // Exists on the game's stage
    public class Position : Component
    {
        Point position;
    }

    public class Health : Component
    {
        int health;
        int baseHealth;

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
        }
    }

    // Can act each turn
    public class Actor : Component
    {
        int energy;
        int energyPerTurn;

        public Actor()
        {
            energy = 0;
            energyPerTurn = 1;
        }

        public Actor(int startingEnergy)
        {
            energy = startingEnergy;
        }

        public bool CanAct
        {
            get { return energy > 0; }
        }

        public void StartTurn()
        {
            energy += energyPerTurn;
        }

        public void Act()
        {

        }
    }

    public class Player : Component
    {

    }

    public class Hostile : Component
    {

    }
}
