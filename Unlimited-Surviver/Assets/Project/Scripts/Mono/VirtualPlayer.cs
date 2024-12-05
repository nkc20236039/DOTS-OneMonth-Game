using DOTS;
using Unity.Entities;
using UnityEngine;

namespace DOTStoMono
{
    public class VirtualPlayer : MonoBehaviour
    {
        private VirtualPlayerManagedSingleton virtualPlayerPosition;

        private void Start()
        {
            virtualPlayerPosition = new VirtualPlayerManagedSingleton
            {
                VirtualPlayerTransform = transform
            };

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            entityManager.CreateSingleton(virtualPlayerPosition);
        }

        private void Update()
        {
            transform.position = virtualPlayerPosition.VirtualPlayerTransform.position;
            transform.rotation = virtualPlayerPosition.VirtualPlayerTransform.rotation;
        }
    }
}