using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean
{
    public class GameEngine
    {
        public string seed;

        public Architect architect;

        public Stage world;
        public Stage CurrentStage
        {
            get { return world; }
        }

        public List<Entity> entities = new List<Entity>();
        public List<GameSystem> systems = new List<GameSystem>();

        public GameEngine()
        {
            seed = GetNewSeed();
        }

        public GameEngine(string seed)
        {
            this.seed = seed;
        }

        public void Initialize()
        {
            // Load systems
            // The order here matters, as this is the order they
            // will run their operations.
            AttachSystem(new PlayerInputSystem());
            AttachSystem(new HostileSystem());
            AttachSystem(new TurnSystem());
            AttachSystem(new PhysicsSystem());
            AttachSystem(new RenderSystem());

            // Load the stage
            architect = new Architect();
            GenerateWorld();

            // Load the player
            entities.Add(EntityFactory.CreatePlayer(this));

            // Load additional entities
            entities.Add(EntityFactory.CreateGoblin(this));
        }

        public void AttachSystem(GameSystem system)
        {
            system.Bind(this);
            systems.Add(system);
        }

        public string GetNewSeed()
        {
            return DateTime.Now.ToString("yyyyMMddTHH:mm:ss.ffff");
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
