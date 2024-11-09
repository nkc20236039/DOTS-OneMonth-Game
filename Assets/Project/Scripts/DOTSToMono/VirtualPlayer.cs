using DOTS;
using Unity.Entities;
using UnityEngine;

namespace DOTStoMono
{
    public class VirtualPlayer : MonoBehaviour
    {
        private VirtualPlayerManagedComponent virtualPlayerPosition;

        private void Start()
        {
            virtualPlayerPosition = new VirtualPlayerManagedComponent
            {
                VirtualPlayerTransform = transform
            };

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            entityManager.CreateSingleton(virtualPlayerPosition);
        }

        private void Update()
        {
            transform.position = virtualPlayerPosition.VirtualPlayerTransform.position;
        }
    }
}