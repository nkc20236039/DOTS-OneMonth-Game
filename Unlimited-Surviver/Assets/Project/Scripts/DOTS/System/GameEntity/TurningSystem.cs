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
                if (math.distancesq(fighterTilt.ValueRO.TargetTurnDirection, float3.zero) == 0) { continue; }
                
                continue;
                // 必要な方向を取得
                var currentForward = math.forward(transform.ValueRO.Rotation);
                var currentRight = math.mul(transform.ValueRO.Rotation, new float3(1, 0, 0));
                var targetDirection = math.normalize(fighterTilt.ValueRO.TargetTurnDirection);

                var angleBetween = SignedAngle(currentForward, targetDirection, math.forward());

                // 機体を傾ける角度を計算
                var maxTiltAngle = fighterParameter.ValueRO.MaxTiltAngle;
                float tiltAngle = math.clamp(-angleBetween, -maxTiltAngle, maxTiltAngle);

                // 傾き用の回転を計算
                quaternion tiltRotation = quaternion.AxisAngle(currentForward, tiltAngle);

                // 新しい回転を計算
                var lookRotation = quaternion.LookRotationSafe(targetDirection, math.up());
                quaternion targetRotation = math.mul(lookRotation, tiltRotation);

                // スムーズに回転させる
                transform.ValueRW.Rotation = math.slerp
                (
                    transform.ValueRO.Rotation,
                    targetRotation,
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