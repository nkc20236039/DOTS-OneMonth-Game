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
        [SerializeField]
        private float rotationSpeed; // 回転の速度
        [SerializeField]
        private float avoidPower;   // 回避力
        [SerializeField]
        private float avoidingTime; // 回避が有効な時間

        private class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity player = GetEntity(TransformUsageFlags.None);

                // プレイヤーにコンポーネントを追加
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