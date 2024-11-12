using DOTStoMono;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    public class GunAiming : MonoBehaviour
    {
        [SerializeField]
        private Camera viewCamera;
        [SerializeField]
        private LayerMask groundLayer;

        private TargetPointManagedSingleton targetPoint;

        private void Start()
        {
            targetPoint = new TargetPointManagedSingleton();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            entityManager.CreateSingleton(targetPoint);
        }

        private void Update()
        {
            var pointer = Pointer.current;
            if (pointer == null)
            {
                return;
            }
            var ray = viewCamera.ScreenPointToRay(pointer.position.ReadValue());
            RaycastHit hitInfo;
            var isRaycastHit = Physics.Raycast(ray, out hitInfo, float.MaxValue, groundLayer);

            if (isRaycastHit)
            {
                targetPoint.Position = hitInfo.point;
            }
        }
    }
}