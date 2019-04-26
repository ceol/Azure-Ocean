using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace AzureOcean
{
    public class GameState
    {
        public string seed;

        public WorldArchitect architect;

        public Stage world;
        public Stage CurrentStage
        {
            get { return world; }
        }

        public List<Entity> entities = new List<Entity>();
        public List<GameSystem> systems = new List<GameSystem>();

        public GameState()
        {
            seed = GetNewSeed();
        }

        public GameState(string seed)
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
            architect = new WorldArchitect();
            GenerateWorld();

            // Load the player
            AttachEntity(EntityFactory.CreatePlayer(this));

            // Load additional entities
            AttachEntity(EntityFactory.CreateGoblin(this));
        }

        public void AttachSystem(GameSystem system)
        {
            system.Bind(this);
            systems.Add(system);
        }

        public void AttachEntity(Entity entity)
        {
            if (CurrentStage != null)
                CurrentStage.SetOccupant(entity);
            entities.Add(entity);
        }

        public void DestroyEntity(Entity entity)
        {
            entities.Remove(entity);
            CurrentStage.RemoveEntity(entity);
        }

        public string GetNewSeed()
        {
            return DateTime.Now.ToString("yyyyMMddTHH:mm:ss.ffff");
        }

        public void GenerateWorld()
        {
           world = architect.GenerateWorld(200, 100, seed);
        }

        public void GenerateNewWorld()
        {
            seed = GetNewSeed();
            GenerateWorld();
        }

        public List<Entity> GetEntities_(Type[] componentTypes)
        {
            List<Entity> filteredEntities = new List<Entity>();
            foreach (Entity entity in entities)
            {
                if (entity.HasComponents(componentTypes))
                    filteredEntities.Add(entity);
            }

            return filteredEntities;
        }

        public List<Entity> GetEntities<T>()
        {
            Type[] types = GetTypesFromFields(typeof(T).GetFields());
            List<Entity> filteredEntities = new List<Entity>();
            foreach (Entity entity in entities)
            {
                if (entity.HasComponents(types))
                    filteredEntities.Add(entity);
            }
            return filteredEntities;
        }

        public Type[] GetTypesFromFields(FieldInfo[] fields)
        {
            Type[] types = new Type[fields.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = fields[i].FieldType;
            }
            return types;
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
