using System.Collections.Generic;

namespace AzureOcean.Components
{
    // Can be manipulated by the inventory
    public class Item : Component
    {
        public int value = 0;
    }

    // Can be used by an actor a certain number of times
    public class Consumable : Component
    {
        public int remaining;
    }

    // Can fill a slot on a character
    public class Equipment : Component
    {
        public int attack = 0;
        public int defense = 0;
        public int speed = 0;
    }

    public enum DamageType { Blunt, Slashing, Piercing }
    // Can be used to deal damage
    public class Weapon : Component
    {
        public DamageType damage;
    }

    // Can be affected by elemental calculations
    public enum ElementType { Fire, Water, Air }
    public class Element : Component
    {
        public ElementType type;
    }
}
