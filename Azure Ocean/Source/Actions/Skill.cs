using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AzureOcean.Actions;
using AzureOcean.Components;

namespace AzureOcean.Skills
{
    public enum Element { Fire, Water, Air }
    public enum Targeting { Single, Multiple, All, Self }

    // Can be randomized and attached to gear
    public class Skill
    {
        public int power;
        public Element element;
        public Targeting targeting;
    }
}
