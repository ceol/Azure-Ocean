using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Components;

namespace AzureOcean
{
    public class Entity
    {
        public Game game;
        public string name;

        List<Component> components = new List<Component>();
        List<Type> componentTypes = new List<Type>();

        public Entity()
        {

        }

        public Entity(string name)
        {
            this.name = name;
        }

        public void Bind(Game game)
        {
            this.game = game;
        }

        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                    return components[i] as T;
            }

            return null;
        }

        public void AttachComponent(Component component)
        {
            component.Bind(this);
            componentTypes.Add(component.GetType());
            components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            component.Bind(null);
            componentTypes.Remove(component.GetType());
            components.Remove(component);
        }

        public void PropagateEvent(GameEvent gameEvent)
        {
            foreach (Component component in components)
            {
                component.ReceiveEvent(gameEvent);
            }
        }

        public bool HasComponents(Type[] types)
        {
            foreach (Type type in types)
            {
                if (!componentTypes.Contains(type))
                    return false;
            }

            return true;
        }
    }

    public static class EntityFactory
    {
        public static Entity CreatePlayer(GameEngine game)
        {
            return CreateEntity("Player",
                new Component[] {
                    new Player(),
                    new Transform(new Vector(20, 20)),
                    new Health(30),
                    new Actor(game),
                    new Render("Images/elf"),
                }
            );
        }

        public static Entity CreateGoblin(GameEngine game)
        {
            return CreateEntity("Goblin",
                new Component[] {
                    new Hostile(),
                    new Transform(new Vector(21, 21)),
                    new Health(30),
                    new Actor(game),
                    new Render("Images/elf"),
                }
            );
        }

        public static Entity CreateEntity(string name, Component[] components)
        {
            Entity entity = new Entity(name);
            foreach (Component component in components)
            {
                entity.AttachComponent(component);
            }
            return entity;
        }
    }
}
