using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    [BurstCompile]
    public partial struct FighterSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<FighterParameterComponent>();
            state.RequireForUpdate<FighterTiltComponent>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var transform, var fighterParameter, var fighterTilt) in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<FighterParameterComponent>,
                RefRW<FighterTiltComponent>>())
            {
                // 傾ける力を-1から1までに制限
                var tiltPower = math.clamp(fighterTilt.ValueRO.TiltPower, -1, 1);

                // 最大角度までの強度に応じて角度を取得
                var angle = fighterParameter.ValueRO.MaxTiltAngle * tiltPower;

                // スムーズに回転させる
                transform.ValueRW.Rotation = math.slerp
                (
                    transform.ValueRO.Rotation,
                    quaternion.Euler(0, 0, -angle),
                    fighterParameter.ValueRO.TiltSpeed * SystemAPI.Time.DeltaTime
                );
            }
        }

        /// <summary>
        /// 符号付き角度を取得
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float SignedAngle(float3 from, float3 to, float3 axis)
        {
            float3 cross = math.cross(from, to);
            float dot = math.dot(from, to);
            float sign = math.sign(math.dot(axis, cross));
            float angle = math.atan2(math.length(cross), dot);

            return angle * sign * math.TODEGREES;
        }
    }
}