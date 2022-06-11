using System;
using System.Collections.Generic;

namespace _Game.Scripts.ModelV4.ECS {
    public class Entity : IEntity {
        private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public int Id { get; }
        public IReadOnlyCollection<IComponent> Components => _components.Values;

        public Entity(int id, params Func<Entity, IComponent>[] componentInstantiators) {
            Id = id;
            foreach (var componentInstantiator in componentInstantiators) {
                var component = componentInstantiator(this);
                var componentType = component.GetType();
                if (_components.ContainsKey(componentType)) {
                    throw new Exception($"Trying to add component of type \"{componentType}\" " +
                                        $"to the entity with id {id} and type \"{GetType()}\" for the second time");
                }

                _components.Add(componentType, component);
            }
        }

        public Component<TComponentData> GetComponent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return _components.TryGetValue(typeof(TComponent), out var component)
                ? (Component<TComponentData>) component
                : null;
        }

        public IComponent<TComponentData> GetModifiableComponent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return GetComponent<TComponent, TComponentData>();
        }

        public IReadOnlyComponent<TComponentData> GetReadOnlyComponent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return GetComponent<TComponent, TComponentData>();
        }
    }
}
