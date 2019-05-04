using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ECS
{
    public class EntityManager
    {
        int entityIncrementor = 0;
        Dictionary<Type, Dictionary<int, object>> componentsByTypeAndEntity;

        public int CreateEntity(object[] components)
        {
            int entityId = entityIncrementor;

            foreach (object component in components)
            {
                Type componentType = component.GetType();
                if (!componentsByTypeAndEntity.ContainsKey(componentType))
                    componentsByTypeAndEntity[componentType] = new Dictionary<int, object>();
                AddComponent(entityId, component);
            }

            entityIncrementor++;
            return entityId;
        }

        public void AddComponent(int entityId, object component)
        {
            componentsByTypeAndEntity[component.GetType()].Add(entityId, component);
        }

        public void RemoveEntity(int entityId)
        {
            foreach (KeyValuePair<Type, Dictionary<int, object>> keyValuePair in componentsByTypeAndEntity)
            {
                if (keyValuePair.Value.ContainsKey(entityId))
                    keyValuePair.Value.Remove(entityId);
            }
        }

        public void RemoveComponent(int entityId, object component)
        {
            componentsByTypeAndEntity[component.GetType()].Remove(entityId);
        }

        public T GetEntity<T>(int entityId)
        {
            T entity = Activator.CreateInstance<T>();

            foreach (FieldInfo field in typeof(T).GetFields())
            {
                Type componentType = field.FieldType;

                if (!componentsByTypeAndEntity.ContainsKey(componentType) || !componentsByTypeAndEntity[componentType].ContainsKey(entityId))
                    return default(T);

                field.SetValue(entity, componentsByTypeAndEntity[componentType][entityId]);
            }

            return entity;
        }

        int CountComponents(Type type)
        {
            return componentsByTypeAndEntity[type].Count;
        }

        void SelectionSortByCount(ref Type[] types)
        {
            Type temp;
            int smallestIndex;
            int typeLength = types.Length;

            for (int i = 0; i < typeLength - 1; i++)
            {
                smallestIndex = i;
                for (int j = i + 1; j < typeLength; j++)
                {
                    if (!componentsByTypeAndEntity.ContainsKey(types[j]) || CountComponents(types[j]) < CountComponents(types[smallestIndex]))
                        smallestIndex = j;
                }
                temp = types[smallestIndex];
                types[smallestIndex] = types[i];
                types[i] = temp;
            }
        }

        public Dictionary<int, T> GetEntities<T>()
        {
            Dictionary<int, T> entities = new Dictionary<int, T>();

            // Create array of component types
            FieldInfo[] componentFields = typeof(T).GetFields();
            Type[] types = new Type[componentFields.Length];
            for (int i = 0; i < componentFields.Length; i++)
                types[i] = componentFields[i].FieldType;
            SelectionSortByCount(ref types);

            // Check each component list by its type
            // Total number of entities is the smallest component count.
            // Only need to loop over one component list, since if an entity
            // doesn't exist in that one, we don't want it anyway.
            foreach (KeyValuePair<int, object> entityComponentPair in componentsByTypeAndEntity[types[0]])
            {
                int entityId = entityComponentPair.Key;

                List<object> components = new List<object>() { entityComponentPair.Value };

                foreach (Type type in types.Skip(1))
                {
                    if (componentsByTypeAndEntity[type].ContainsKey(entityId))
                        components.Add(componentsByTypeAndEntity[type][entityId]);
                }

                if (components.Count == types.Length)
                {
                    T entity = Activator.CreateInstance<T>();
                    foreach (object component in components)
                    {
                        foreach (FieldInfo componentField in componentFields)
                        {
                            if (component.GetType() == componentField.FieldType)
                            {
                                componentField.SetValue(entity, component);
                                break;
                            }
                        }
                    }
                }
            }

            return entities;
        }

        public T CreateInstance<T>(List<object> components)
        {
            T instance = Activator.CreateInstance<T>();

            return instance;
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
    }

    public abstract class System
    {
        protected EntityManager entityManager;

        public void Bind(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public virtual void Initialize()
        {

        }

        public abstract void Run();
    }
}
