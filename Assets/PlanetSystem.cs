using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlanetSystem : JobComponentSystem {

	ComponentGroup allPlanets;
	static GameObject sun;

	protected void OnCreateManager(int capacity){
		allPlanets = GetComponentGroup(typeof(Position),typeof(Heading),typeof(MoveSpeed)); 
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitWithScene(){
		sun = GameObject.Find("Sun");
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps){

		var positions = allPlanets.GetComponentDataArray<Position>();
		var headings = allPlanets.GetComponentDataArray<Heading>();
		var speeds = allPlanets.GetComponentDataArray<MoveSpeed>();

		var steerJob = new Steer{
			headings = headings,
			planetPositions = positions,
			speeds = speeds,
			sunPosition = sun.transform.position
		};

		var steerJobHandle = steerJob.Schedule(allPlanets.CalculateLength(),64);
		steerJobHandle.Complete();
		inputDeps = steerJobHandle;
		return inputDeps;
	}

}

