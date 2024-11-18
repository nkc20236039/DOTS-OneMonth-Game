using DOTS;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DamagePresenter
{
    private EntityManager entityManager;
    private EntityQuery query;
    public DamagePresenter()
    {
        // DOTS空間からコンポーネントを取得
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        query = entityManager.CreateEntityQuery(typeof(DamageManagedSingleton));
        
    }

    public void Show()
    {
        
    }
}
