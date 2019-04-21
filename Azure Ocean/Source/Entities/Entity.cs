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

        public Component GetComponent<T>()
        {
            foreach (Component component in components)
            {
                if (component is T)
                    return component;
            }

            return null;
        }

        public void AttachComponent(Component component)
        {
            component.entity = this;
            componentTypes.Add(component.GetType());
            components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            component.entity = null;
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
        public static Entity CreatePlayer()
        {
            Entity entity = new Entity("Player");
            entity.AttachComponent(new Player());
            entity.AttachComponent(new Position());
            entity.AttachComponent(new Energy());
            entity.AttachComponent(new Health(30));
            return entity;
        }

        public static Entity CreateGoblin()
        {
            Entity entity = new Entity("Goblin");
            entity.AttachComponent(new Hostile());
            entity.AttachComponent(new Position());
            entity.AttachComponent(new Health(5));
            return entity;
        }
    }
}
