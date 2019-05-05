using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean.Components
{
    public class Render : Component
    {
        public string content;

        public Render(string content)
        {
            this.content = content;
        }
    }
}
