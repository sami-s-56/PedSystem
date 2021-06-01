using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SignalState
{
	Red,
	Green
}

public class Signal : MonoBehaviour {

	//public GameObject crossing;	//Crossing Prefab with 4 waypoints, 2 for each side (to and fro)

	[SerializeField] Waypoint[] controlPoints;	//Array of crossing waypoints (Branches) to control

	public float initialDelay;	//Delay after which signal should start operating (In case it is purely timer based)
	public float activeTime;	//GreenSignalTime
	public float inactiveTime;  //RedSignalTime

	[SerializeField] SignalState signalState;

	[SerializeField] bool isStateBased;	//if checked, will depend on external trigger to work. SetSignal() should be used
	[SerializeField] bool useTimer; //if checked, will use seperate timer for changing states, if statebased is checked alongwith, then timer will start after 1st trigger

	void Start()
	{
		//If not using external colliders to Set Signal then start via timer
		if(!isStateBased)
			Invoke("Activate", initialDelay);

		
	}

	void Activate()
	{
		foreach (Waypoint point in controlPoints)
		{
			point.canCross = true;
		}

		signalState = SignalState.Green;
		ChangeColor(Color.green);

		if (!useTimer)
			return;

		Invoke("Deactivate", activeTime);
	}

	void Deactivate()
	{
		foreach (Waypoint point in controlPoints)
		{
			point.canCross = false;
		}

		signalState = SignalState.Red;
		ChangeColor(Color.red);

		if (!useTimer)
			return;

		Invoke("Activate", inactiveTime);
	}

	public void SetSignal(SignalState state)
	{
		if(state == SignalState.Green)
		{
			Activate();
		}
		if(state == SignalState.Red)
		{
			Deactivate();
		}
	}

	void ChangeColor(Color color)
	{
		GetComponent<MeshRenderer>().material.color = color;
	}
}
