using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct ExperienceOrbSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {

        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            var playerEntity = SystemAPI.GetSingletonEntity<PlayerSingleton>();
            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);

            new ExperienceOrbJob
            {
                PlayerPosition = playerTransform.ValueRO.Position
            }.ScheduleParallel();
        }
    }

    public partial struct ExperienceOrbJob : IJobEntity
    {
        public float3 PlayerPosition;

        private void Execute(
            in ExperienceOrbComponent expOrb,
            in LocalTransform transform,
            ref PhysicsVelocity velocity,
            ref PhysicsMass physicsMass)
        {
            // 回転物理計算をキャンセル
            physicsMass.InverseInertia = 0;

            var rawDirection = transform.Position - PlayerPosition;
            // プレイヤーとの距離を計算
            var distance = math.distancesq(transform.Position, PlayerPosition);   // 軽量化のためにルート計算を省略

            if (expOrb.AttractedRange * 2 < distance) { return; }

            // 近づくにつれて早くなるように速度を計算
            var speed = (distance - expOrb.AttractedRange * 2) * expOrb.AttractedSpeed;
            velocity.Linear = math.normalize(rawDirection) * speed;
        }
    }
}
