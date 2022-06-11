using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public interface IComponentUpdateProxy {
        void RegisterComponent<TComponentData>(Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData>;
        void UnregisterComponent<TComponentData>(Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData>;
    }
}
