using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class WaypointTool : EditorWindow
{
	[MenuItem("Tools/Waypoint Editor")]
	public static void Open()
	{
		GetWindow<WaypointTool>();
	}

	[SerializeField] Transform wpNetworkHolder;
	[SerializeField] Transform waypointRoot;

	[SerializeField]
	GameObject activeObject;

	[SerializeField] Waypoint previousSelect;
	[SerializeField] Waypoint currentSelect;

	void OnGUI()
	{
		SerializedObject obj = new SerializedObject(this);

		EditorGUILayout.PropertyField(obj.FindProperty("wpNetworkHolder"));
		EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));
		EditorGUILayout.PropertyField(obj.FindProperty("activeObject"));
		EditorGUILayout.PropertyField(obj.FindProperty("previousSelect"));
		EditorGUILayout.PropertyField(obj.FindProperty("currentSelect"));

		EditorGUILayout.BeginVertical("box");
		DrawButtons();



		obj.ApplyModifiedProperties();
	}

	void OnSelectionChange()
	{
		Debug.Log("Selection Changed");

		activeObject = Selection.activeGameObject;

		if(activeObject != null && activeObject.GetComponent<PedSpawner>())
		{
			wpNetworkHolder = activeObject.transform;
		}

		if (activeObject != null && activeObject.GetComponent<BlockScript>())
		{
			waypointRoot = activeObject.transform;
			wpNetworkHolder = activeObject.transform.parent;
		}
		else if (activeObject != null && activeObject.GetComponent<Waypoint>())
		{
			wpNetworkHolder = activeObject.transform.parent.parent;
			waypointRoot = activeObject.transform.parent;
			
			if (Selection.gameObjects.Length > 1 && currentSelect != null)
			{
				previousSelect = currentSelect;
			}
			currentSelect = activeObject.GetComponent<Waypoint>();
			
		}
		else
		{
			currentSelect = null;
			previousSelect = null;
		}

		if (Selection.gameObjects.Length <= 1)
			previousSelect = null;
		

		Repaint();

	}

	void DrawButtons()
	{

		//Button to create an area (or a group of waypoint networks with assigned spawner script)
		if(GUILayout.Button("New Area"))
		{
			AddArea();
		}

		//Button for creating a WaypointNetwork 
		if(waypointRoot == null || currentSelect==null)
		{
			if(GUILayout.Button("New Waypoint Network"))
			{
				AddNewNetwork();
			}
		}

		//Button to create first waypoint of any network (One whcih is not linked to any)
		if(waypointRoot!=null && currentSelect == null)
		{
			if (GUILayout.Button("Create Waypoint"))
			{
				//Debug.Log("Creating new waypoint");
				CreateWaypoint();
			}
		}

		//Buttons to create waypoint before / after the selected waypoint or delete the waypoint
		if(Selection.activeGameObject!=null && Selection.activeGameObject.GetComponent<Waypoint>())
		{
			if (GUILayout.Button("Create Waypoint Before"))
			{
				CreateWaypointBefore();
			}
			if (GUILayout.Button("Create Waypoint After"))
			{
				CreateWaypointAfter();
			}
			if (GUILayout.Button("Delete Waypoint"))
			{
				DeleteWaypoint();
			}
			if(GUILayout.Button("Add Branch"))
			{
				AddBranch();
			}
		}

		//Button to create crossing connection between 2 compatible waypoints
		if(previousSelect != null && currentSelect != null)
		{
			if((previousSelect.isBranch && !currentSelect.isBranch) || (!previousSelect.isBranch && currentSelect.isBranch))
			{
				if (GUILayout.Button("Create Crossing"))
				{
					CreateCrossing();
				}
			}
		}

		if(currentSelect != null && currentSelect.isBranch && currentSelect.nextWaypoint != null)
		{
			if (GUILayout.Button("Remove Crossing Connection"))
			{
				RemoveCrossing();
			}
		}
	}

	private void AddArea()
	{
		GameObject obj = new GameObject("WaypointArea" + wpNetworkHolder.childCount + 1);
		obj.transform.position = new Vector3(0f, 0f, 0f);
		obj.AddComponent<PedSpawner>();
		Selection.activeObject = obj;
	}

	private void AddNewNetwork()
	{
		if(wpNetworkHolder == null)
		{
			AddArea();
		}

		GameObject obj = new GameObject("WaypointNetwork" + wpNetworkHolder.childCount+1);
		obj.transform.parent = wpNetworkHolder;
		obj.transform.position = new Vector3(0f,0f,0f);
		obj.AddComponent<BlockScript>();
		Selection.activeObject = obj;

	}

	void RemoveCrossing()
	{
		currentSelect.nextWaypoint = null;
	}

	void CreateCrossing()
	{
		if (previousSelect.isBranch)
		{
			if(previousSelect.nextWaypoint != null)
			{
				Debug.LogWarning("Crossing from this branch already exists!");
			}
			else
			{
				previousSelect.nextWaypoint = currentSelect;
			}
		}
		else
		{
			if (currentSelect.nextWaypoint != null)
			{
				Debug.LogWarning("Crossing from this branch already exists!");
			}
			else
			{
				currentSelect.nextWaypoint = previousSelect;
			}
		}
	}

	void AddBranch()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
		
		Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
		if(branchedFrom == null)
		{
			Debug.Log("Problem with:" + Selection.activeGameObject.name);
		}
		else
		{
			Debug.Log("BranchFrom:" + branchedFrom.name);
		}
		branchedFrom.branches.Add(waypoint);

		waypoint.previousWaypoint = branchedFrom;
		waypoint.isBranch = true;

		waypoint.transform.position = branchedFrom.transform.position;
		waypoint.transform.forward = branchedFrom.transform.forward;

		Selection.activeGameObject = waypoint.gameObject;
	}

	void CreateWaypoint()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
		if (waypointRoot.childCount > 1)
		{
			waypoint.previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
			waypoint.previousWaypoint.nextWaypoint = waypoint;

			waypoint.transform.position = waypoint.previousWaypoint.transform.position;
			waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
		}

		Selection.activeGameObject = waypoint.gameObject;
	}

	void CreateWaypointBefore()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
		Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

		newWaypoint.transform.position = selectedWaypoint.transform.position;
		newWaypoint.transform.forward = selectedWaypoint.transform.forward;

		if(selectedWaypoint.previousWaypoint != null)
		{
			newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
			newWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
		}

		selectedWaypoint.previousWaypoint = newWaypoint;
		newWaypoint.nextWaypoint = selectedWaypoint;

		newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

		Selection.activeGameObject = newWaypoint.gameObject;
	}

	void CreateWaypointAfter()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
		Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

		newWaypoint.transform.position = selectedWaypoint.transform.position;
		newWaypoint.transform.forward = selectedWaypoint.transform.forward;

		if (selectedWaypoint.nextWaypoint != null)
		{
			newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
			newWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
		}

		selectedWaypoint.nextWaypoint = newWaypoint;
		newWaypoint.previousWaypoint = selectedWaypoint;

		newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

		Selection.activeGameObject = newWaypoint.gameObject;
	}

	void DeleteWaypoint()
	{
		Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
		
		if (selectedWaypoint.isBranch)
		{
			selectedWaypoint.previousWaypoint.branches.Remove(selectedWaypoint);
		}
		else
		{
			if (selectedWaypoint.previousWaypoint != null)
			{
				selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
			}
			if (selectedWaypoint.nextWaypoint != null)
			{
				selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
			}
		}
		

		DestroyImmediate(selectedWaypoint.gameObject);
	}
}
