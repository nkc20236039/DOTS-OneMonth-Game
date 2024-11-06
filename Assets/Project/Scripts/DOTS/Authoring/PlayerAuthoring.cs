using DOTS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Mono
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float speed;    // プレイヤーの速度

        private class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity player = GetEntity(TransformUsageFlags.None);

                // プレイヤーにコンポーネントを追加
                AddComponent(player, typeof(MovementComponent));
                AddComponent(player, new PlayerComponent
                {
                    Speed = authoring.speed
                });
            }
        }
    }
}