using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerCollider : MonoBehaviour {

	[SerializeField] PedSpawner pedSpawner;

	void Start()
	{
		Invoke("EnableTrigger", 3f);
	}

	void EnableTrigger()
	{
		GetComponent<SphereCollider>().enabled = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if(Time.time > 5f)
		{
			if (pedSpawner.spawnPoints.Contains(other.GetComponent<Waypoint>()))
			{
				pedSpawner.spawnPoints.Remove(other.GetComponent<Waypoint>());
			}
		}
	}
}
