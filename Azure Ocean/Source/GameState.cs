using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

using ECS;
using AzureOcean.Systems;
using AzureOcean.Components;

namespace AzureOcean
{
    public class GameState
    {
        public string seed;

        public WorldArchitect architect;

        public Stage world;
        public Stage board;
        public Stage CurrentStage
        {
            get
            {
                if (board != null)
                    return board;
                return world;
            }
        }

        public bool inCombat = false;

        public EntityManager entityManager = new EntityManager();
        public List<ECS.System> systems = new List<ECS.System>();

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
            AttachSystem(new HostileTrackingSystem());
            AttachSystem(new MovementSystem());

            // Load the stage
            architect = new WorldArchitect();
            GenerateWorld();

            // Create entities
            entityManager.CreateEntity(new object[]
            {
                new Player(),
                new Transform(new Vector(30, 30)),
                new Render("Images/elf"),
            });

            entityManager.CreateEntity(new object[]
            {
                new Hostile("Goblin"),
                new Transform(new Vector(50, 50)),
                new Render("Images/elf"),
            });
        }

        public void AttachSystem(ECS.System system)
        {
            system.Bind(entityManager);
            systems.Add(system);
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

        public void Update()
        {
            // loop through all non-render systems?
            foreach (ECS.System system in systems)
            {
                system.Run();
            }
        }
    }
}
