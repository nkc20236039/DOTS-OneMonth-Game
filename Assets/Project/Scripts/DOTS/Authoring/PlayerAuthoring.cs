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

        private class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity player = GetEntity(TransformUsageFlags.None);

                // �v���C���[�ɃR���|�[�l���g��ǉ�
                AddComponent(player, typeof(MovementComponent));
                AddComponent(player, new PlayerComponent
                {
                    Speed = authoring.speed
                });
            }
        }
    }
}