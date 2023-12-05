using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace _Game.Scripts.ModelV4.ECS.Systems {
    public abstract class System {
        private readonly List<IEntity> _trackedEntities = new List<IEntity>();
        protected IReadOnlyCollection<IEntity> TrackedEntities => _trackedEntities;
        protected GameDataAPI GameAPI { get; private set; }

        public void Initialize(GameDataAPI gameAPI, IEnumerable<IEntity> entities) {
            GameAPI = gameAPI;
            foreach (var entity in entities) {
                TryAddEntity(entity);
            }
        }

        public void TryAddEntity(IEntity entity) {
            if (!_trackedEntities.Contains(entity) && EntitySelector(entity)) {
                _trackedEntities.Add(entity);
                OnEntityAdded(entity);
            }
        }

        public void TryRemoveEntity(IEntity entity) {
            if (_trackedEntities.Remove(entity)) {
                OnEntityRemoved(entity);
            }
        }

        private bool EntitySelector(IEntity entity) {
            return GetComponents(entity).All(c => c != null);
        }

        [ItemCanBeNull]
        protected abstract IEnumerable<IComponent> GetComponents(IEntity entity);

        protected virtual void OnEntityAdded(IEntity entity) {}
        protected virtual void OnEntityRemoved(IEntity entity) {}
    }
}