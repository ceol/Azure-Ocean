using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean
{
    public class Game
    {
        public string seed;

        public Architect architect;

        public Stage world;
        public Stage currentStage;

        public List<Entity> entities = new List<Entity>();
        public List<GameSystem> systems = new List<GameSystem>();

        public Game()
        {
            seed = GetNewSeed();
        }

        public Game(string seed)
        {
            this.seed = seed;
        }

        public void Initialize()
        {
            // Load systems
            // The order here matters, as this is the order they
            // will run their operations.
            systems.Add(new ActorSystem(this));

            entities.Add(EntityFactory.CreatePlayer());
            entities.Add(EntityFactory.CreateGoblin());

            architect = new Architect();
            GenerateWorld();
        }

        public string GetNewSeed()
        {
            return DateTime.Now.ToString();
        }

        public void GenerateWorld()
        {
           world = architect.GenerateWorld(160, 80, seed);
        }

        public void GenerateNewWorld()
        {
            seed = GetNewSeed();
            GenerateWorld();
        }

        public List<Entity> GetEntities(Type[] componentTypes)
        {
            List<Entity> filteredEntities = new List<Entity>();
            foreach (Entity entity in entities)
            {
                if (entity.HasComponents(componentTypes))
                    filteredEntities.Add(entity);
            }

            return filteredEntities;
        }

        public void Update()
        {
            // loop through all non-render systems?
            foreach (GameSystem system in systems)
            {
                system.Run();
            }
        }
    }
}
