using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Actions
{
    public enum Element { Fire, Water, Air }
    public enum Targeting { Single, Multiple, All, Self }

    public abstract class Effect
    {
        public Element element;
        public Targeting targeting;
        public int duration;

        public abstract void Apply(Entity target);
    }
}
