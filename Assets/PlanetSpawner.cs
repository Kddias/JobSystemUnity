using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class PlanetSpawner {

    static EntityManager planetManager;
    static MeshInstanceRenderer planetRenderer;
    static EntityArchetype planetArchetype;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        planetManager = World.Active.GetOrCreateManager<EntityManager>();
        planetArchetype = planetManager.CreateArchetype(typeof(Position),
                                                        typeof(Unity.Transforms.LocalPosition),
                                                        typeof(MoveForward), //required for 3D
                                                        typeof(TransformMatrix),
                                                        typeof(MoveSpeed),
                                                        typeof(Rigidbody));
       
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitWithScene()
    {
        planetRenderer = GameObject.FindObjectOfType<MeshInstanceRendererComponent>().Value;
        for (int i = 0; i < 40000; i++)
        {
            SpawnPlanet();
        }

    }

    static void SpawnPlanet()
    {
        Entity planetEntity = planetManager.CreateEntity(planetArchetype);
        Vector3 pos = UnityEngine.Random.insideUnitSphere * 100;
        planetManager.SetComponentData(planetEntity, new LocalPosition { Value = new float3(pos.x, 0, pos.z) });
        planetManager.SetComponentData(planetEntity, new Heading { Value = new float3(1,0,0) });
        planetManager.SetComponentData(planetEntity, new MoveSpeed { speed = 15f});

        planetManager.AddSharedComponentData(planetEntity, planetRenderer);
        //planetManager.AddSharedComponentData(planetEntity, new Rigidbody());
    }




}
