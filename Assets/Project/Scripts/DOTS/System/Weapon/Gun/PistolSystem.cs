using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS
{
    public partial struct PistolSystem : ISystem
    {
        void ISystem.OnCreate(ref Unity.Entities.SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<PistolComponent, LocalTransform, WeaponComponent>()
                .Build();

            state.RequireForUpdate(query);
        }

        void ISystem.OnUpdate(ref Unity.Entities.SystemState state)
        {
            foreach ((var pistol, var weapon, var transform) in SystemAPI.Query<
                RefRW<PistolComponent>,
                RefRO<WeaponComponent>,
                RefRO<LocalTransform>>())
            {
                // クールダウンの判定
                pistol.ValueRW.Cooldown += SystemAPI.Time.DeltaTime;
                if (pistol.ValueRO.ShotInterval > pistol.ValueRO.Cooldown) { return; }

                // 弾を召喚
                var bullet = state.EntityManager.Instantiate(pistol.ValueRO.Bullet);

                // オフセットを適用
                var offsetDirection
                    = math.forward(weapon.ValueRO.WorldRotation)
                    * pistol.ValueRO.Offset.x;
                var position = weapon.ValueRO.WorldPosition + offsetDirection;
                position.y += pistol.ValueRO.Offset.y;

                state.EntityManager.SetComponentData(bullet, new LocalTransform
                {
                    Position = position,
                    Scale = 1,
                    Rotation = weapon.ValueRO.WorldRotation
                });

                // 時間を初期化
                pistol.ValueRW.Cooldown = 0;
            }
        }
    }
}