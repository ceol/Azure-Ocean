﻿using Microsoft.Xna.Framework;
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
            if (types == null || types.Length == 0)
                return false;

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
        public static Entity CreatePlayer(GameState game)
        {
            return CreateEntity("Player",
                new Component[]
                {
                    new Player(),
                    new Transform(new Vector(30, 30)),
                    new Health(30),
                    new Actor(game),
                    new Render("Images/elf"),
                }
            );
        }

        public static Entity CreateGoblin(GameState game)
        {
            return CreateEntity("Goblin",
                new Component[]
                {
                    new Hostile(),
                    new Transform(new Vector(60, 40)),
                    new Health(30),
                    new Actor(game),
                    new Render("Images/elf"),
                }
            );
        }

        public static Entity CreateSword()
        {
            return CreateEntity("Adventurer's Sword",
                new Component[]
                {
                    new Item(),
                    new Equipment() { attack = 5 },
                    new Weapon() { damage = DamageType.Slashing },
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
