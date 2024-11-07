using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace DOTS
{
    [UpdateInGroup(typeof(ActionUpdateGroup))]
    [BurstCompile]
    public partial struct AvoidSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<PhysicsVelocity>();
            state.RequireForUpdate<AvoidComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            new AvoidJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct AvoidJob : IJobEntity
    {
        public float DeltaTime;

        public void Execute(ref PhysicsVelocity velocity, ref AvoidComponent avoid)
        {
            // 回避を使用していなければ終了する
            if (!avoid.IsAvoiding) { return; }

            // 回避方向へ力を加算
            velocity.Linear += avoid.AvoidDirection * avoid.AvoidPower * DeltaTime;
            
            // 回避時間の計算
            avoid.AvoidingElapsedTime += DeltaTime;
            if (avoid.AvoidingTime < avoid.AvoidingElapsedTime)
            {
                avoid.IsAvoiding = false;
                avoid.AvoidingElapsedTime = 0;
            }
        }
    }
}