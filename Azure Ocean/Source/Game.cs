using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean
{
    public class Game
    {
        public string seed;

        public Stage world;
        public Stage currentStage;

        public Game()
        {
            seed = GetNewSeed();
        }

        public Game(string seed)
        {
            this.seed = seed;
        }

        public string GetNewSeed()
        {
            return DateTime.Now.ToString();
        }

        public void GenerateWorld()
        {
            Architect architect = new Architect();
            world = architect.GenerateWorld(160, 80, seed);
        }

        public void GenerateNewWorld()
        {
            seed = GetNewSeed();
            GenerateWorld();
        }
    }
}
