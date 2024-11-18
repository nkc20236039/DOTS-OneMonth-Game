using DOTS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float speed;    // �v���C���[�̑��x
        [SerializeField]
        private float rotationSpeed; // ��]�̑��x
        [SerializeField]
        private float avoidPower;   // ����
        [SerializeField]
        private float avoidingTime; // ������L���Ȏ���

        private class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity player = GetEntity(TransformUsageFlags.None);

                // �v���C���[�ɃR���|�[�l���g��ǉ�
                AddComponent(player, typeof(PlayerInputComponent));
                AddComponent(player, new PlayerSingleton
                {
                    Speed = authoring.speed,
                    RotationSpeed = authoring.rotationSpeed,
                });
                AddComponent(player, new AvoidComponent
                {
                    AvoidPower = authoring.avoidPower,
                    AvoidingTime = authoring.avoidingTime
                });
            }
        }
    }
}