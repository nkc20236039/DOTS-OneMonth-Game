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
            foreach ((var pistol, var weapon, var transform, var pistolEntity) in SystemAPI.Query<
                RefRW<PistolComponent>,
                RefRO<WeaponComponent>,
                RefRO<LocalTransform>>()
                .WithEntityAccess())
            {
                // �N�[���_�E���̔���
                pistol.ValueRW.Cooldown += SystemAPI.Time.DeltaTime;
                if (pistol.ValueRO.ShotInterval > pistol.ValueRO.Cooldown) { return; }

                // �e������
                var bullet = state.EntityManager.Instantiate(pistol.ValueRO.Bullet);

                // �I�t�Z�b�g��K�p
                var offsetDirection
                    = math.forward(weapon.ValueRO.WorldRotation)
                    * pistol.ValueRO.Offset.x;
                var position = weapon.ValueRO.WorldPosition + offsetDirection;
                position.y += pistol.ValueRO.Offset.y;

                // �ʒu����������
                state.EntityManager.SetComponentData(bullet, new LocalTransform
                {
                    Position = position,
                    Scale = 1,
                    Rotation = weapon.ValueRO.WorldRotation
                });

                var bulletComponent = SystemAPI.GetComponent<BulletComponent>(bullet);
                // �e�ɂȂ��Ă���(�v���C���[�z��)�G���e�B�e�B���擾
                var parent = SystemAPI.GetComponent<Parent>(pistolEntity);
                // �e�̃I�[�i�[���w��
                bulletComponent.Owner = parent.Value;

                // ���Ԃ�������
                pistol.ValueRW.Cooldown = 0;
            }
        }
    }
}