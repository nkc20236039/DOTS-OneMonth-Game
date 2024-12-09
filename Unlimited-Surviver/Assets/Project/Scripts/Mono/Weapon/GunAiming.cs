using DOTStoMono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mono
{
    public class GunAiming
    {
        private EntityManager entityManager;

        private GunAiming()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Set()
        {
            
        }
    }
}