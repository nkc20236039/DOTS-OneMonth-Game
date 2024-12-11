using DOTS;
using DOTStoMono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Mono
{
    public class TargetPointSubscriber
    {
        private EntityManager entityManager;
        private Entity player;
        public TargetPointSubscriber()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerSingleton>();
            player = entityManager
                .CreateEntityQuery(entityQueryBuilder)
                .GetSingletonEntity();
        }

        public void SetSignedTargetRatio(float control, float pitch)
        {
            entityManager.SetComponentData(player, new TargetPointComponent
            {
                TargetAngle = control,
                Pitch = pitch,
            });
        }
    }
}