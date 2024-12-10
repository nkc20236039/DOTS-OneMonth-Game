using DOTS;
using DOTStoMono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class CrosshairTargetConverter
    {
        private EntityManager entityManager;
        private Entity player;
        public CrosshairTargetConverter()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton>();
            player = entityManager
                .CreateEntityQuery(entityQueryBuilder)
                .GetSingletonEntity();
        }

        public void SetAngle(float angle)
        {
            var targetPoint = entityManager.GetComponentData<TargetPointComponent>(player);
            targetPoint.TargetAngle = angle;
            entityManager.SetComponentData(player, targetPoint);
        }
    }
}