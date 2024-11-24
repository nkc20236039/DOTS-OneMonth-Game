using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct GetExperienceOrbSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<ExperienceOrbComponent>();
            state.RequireForUpdate<LevelSingleton>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var player = SystemAPI.GetSingletonEntity<PlayerSingleton>();

            state.Dependency = new PlayerExpGetJob
            {
                Player = player,
                PlayerTransform = SystemAPI.GetComponent<LocalTransform>(player),
                ParallelEcb = ecb.AsParallelWriter(),
                Level = SystemAPI.GetSingleton<LevelSingleton>(),
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
    }

    [BurstCompile]
    public partial struct PlayerExpGetJob : IJobEntity
    {
        [ReadOnly]
        public Entity Player;
        [ReadOnly]
        public LocalTransform PlayerTransform;
        public LevelSingleton Level;
        public EntityCommandBuffer.ParallelWriter ParallelEcb;

        public void Execute(
            [ChunkIndexInQuery] int index,
            Entity entity,
            in ExperienceOrbComponent expGroup,
            in LocalTransform transform)
        {
            // 取得可能範囲か調べる
            var distance = math.distancesq(transform.Position, PlayerTransform.Position);
            if (Level.GettableRange * 2 < distance) { return; }

            // 経験値を取得している判定であればその分のポイントを加算
            Level.AdditionExp += expGroup.Point;
            ParallelEcb.SetComponent(index, Player, Level);

            ParallelEcb.DestroyEntity(index, entity);
        }
    }
}