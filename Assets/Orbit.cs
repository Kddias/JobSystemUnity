using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class Orbit : MonoBehaviour {

    public GameObject sun;
    GameObject[] planets;
    public int numPlanets = 50;

    Transform[] planetTransforms;
    TransformAccessArray planetTransformsAccessArray;
    PositionUpdateJob planetJob;
    JobHandle planetPositionJobHandle;

	void Start () {
        planets = new GameObject[numPlanets];
        planetTransforms = new Transform[numPlanets];
        for (int i = 0; i < numPlanets; i++)
        {
            planets[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planets[i].GetComponent<Collider>().enabled = false;
            planets[i].transform.position = Random.insideUnitSphere * 50;
            planetTransforms[i] = planets[i].transform;
        }

        planetTransformsAccessArray = new TransformAccessArray(planetTransforms);
	}

    struct PositionUpdateJob : IJobParallelForTransform
    {
        public Vector3 sunPos;
        public void Execute(int i, TransformAccess transform)
        {
            Vector3 direction = (sunPos - transform.position);
            float gravity = Mathf.Clamp(direction.magnitude / 100.0f, 0, 1);
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, gravity);

            float orbitalSpeed = Mathf.Sqrt(100 / direction.magnitude);

            transform.position += transform.rotation * Vector3.forward * orbitalSpeed;
        }
    }

	void Update () 
    {
        planetJob = new PositionUpdateJob()
        {
            sunPos = sun.transform.position
        };

        planetPositionJobHandle = planetJob.Schedule(planetTransformsAccessArray);
	}

    public void LateUpdate()
    {
        planetPositionJobHandle.Complete();
    }

    private void OnDestroy()
    {
        planetTransformsAccessArray.Dispose();
    }
}
