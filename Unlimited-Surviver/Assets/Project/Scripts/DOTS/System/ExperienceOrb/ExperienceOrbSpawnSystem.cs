using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;

namespace DOTS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct ExperienceOrbSpawnSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            state.RequireForUpdate<ExperienceOrbComponent>();
            state.RequireForUpdate<PhysicsVelocity>();
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var expOrb, var velocity, var material, var entity) in SystemAPI.Query<
                RefRW<ExperienceOrbComponent>,
                RefRW<PhysicsVelocity>,
                RefRW<ExperienceMaterialSeedComponent>>()
                .WithEntityAccess())
            {
                // 最初の実行以外は処理をしない
                if (expOrb.ValueRO.IsSpawned) { continue; }

                // 飛ばすランダムな方向を取得
                var random = new Random((uint)entity.Index);
                var dropRange = expOrb.ValueRO.DropAngleRange;
                var randomEuler = new float3
                {
                    x = random.NextFloat(-dropRange, dropRange),
                    y = random.NextFloat(0, 360),
                    z = random.NextFloat(-dropRange, dropRange),
                };

                // 角度をベクトルに変換
                var dropAngle = quaternion.Euler(randomEuler);
                var dropDirection = math.mul(dropAngle, math.up());

                // 物理に反映
                velocity.ValueRW.Linear = math.normalize(dropDirection) * expOrb.ValueRO.DropForce;

                // マテリアルのシードを決める
                material.ValueRW.Value = random.NextFloat2();

                expOrb.ValueRW.IsSpawned = true;
            }
        }
    }
}