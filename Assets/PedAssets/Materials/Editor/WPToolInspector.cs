
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WPTool))]
public class WPToolInspector : Editor {

	WPTool wPTool;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		//wPTool = (WPTool)target;

		EditorGUILayout.BeginVertical("box");
		DrawButtons(wPTool);

	}

	void OnEnable()
	{
		if(wPTool == null)
		{
			wPTool = (WPTool)target;
		}
		

		if (wPTool.isSubscribed == false)
		{
			wPTool.isSubscribed = true;
			Selection.selectionChanged += OnSelectionChange;
		}
			
	}

	void OnSelectionChange()
	{
		Debug.Log("Selection Changed");
		wPTool.isSubscribed = true;
		wPTool.activeObject = Selection.activeGameObject;

		if (wPTool.activeObject != null && wPTool.activeObject.GetComponent<PedSpawner>())
		{
			wPTool.wpNetworkHolder = wPTool.activeObject.transform;
		}

		if (wPTool.activeObject != null && wPTool.activeObject.GetComponent<BlockScript>())
		{
			wPTool.waypointRoot = wPTool.activeObject.transform;
			wPTool.wpNetworkHolder = wPTool.activeObject.transform.parent;
		}
		else if (wPTool.activeObject != null && wPTool.activeObject.GetComponent<Waypoint>())
		{
			wPTool.wpNetworkHolder = wPTool.activeObject.transform.parent.parent;
			wPTool.waypointRoot = wPTool.activeObject.transform.parent;

			if (Selection.gameObjects.Length > 1 && wPTool.currentSelect != null)
			{
				wPTool.previousSelect = wPTool.currentSelect;
			}
			wPTool.currentSelect = wPTool.activeObject.GetComponent<Waypoint>();

		}
		else
		{
			wPTool.currentSelect = null;
			wPTool.previousSelect = null;
		}

		if (Selection.gameObjects.Length <= 1)
			wPTool.previousSelect = null;


		//Repaint();

	}

	void DrawButtons(WPTool wPTool)
	{

		//Button to create an area (or a group of waypoint networks with assigned spawner script)
		if (GUILayout.Button("New Area"))
		{
			 wPTool.AddArea();
		}

		//Button for creating a WaypointNetwork 
		if (wPTool.waypointRoot == null || wPTool.currentSelect == null)
		{
			if (GUILayout.Button("New Waypoint Network"))
			{
				wPTool.AddNewNetwork();
			}
		}

		//Button to create first waypoint of any network (One whcih is not linked to any)
		if (wPTool.waypointRoot != null && wPTool.currentSelect == null)
		{
			if (GUILayout.Button("Create Waypoint"))
			{
				//Debug.Log("Creating new waypoint");
				wPTool.CreateWaypoint();
			}
		}

		//Buttons to create waypoint before / after the selected waypoint or delete the waypoint
		if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
		{
			if (GUILayout.Button("Create Waypoint Before"))
			{
				wPTool.CreateWaypointBefore();
			}
			if (GUILayout.Button("Create Waypoint After"))
			{
				wPTool.CreateWaypointAfter();
			}
			if (GUILayout.Button("Delete Waypoint"))
			{
				wPTool.DeleteWaypoint();
			}
			if (GUILayout.Button("Add Branch"))
			{
				wPTool.AddBranch();
			}
		}

		//Button to create crossing connection between 2 compatible waypoints
		if (wPTool.previousSelect != null && wPTool.currentSelect != null)
		{
			if ((wPTool.previousSelect.isBranch && !wPTool.currentSelect.isBranch) || (!wPTool.previousSelect.isBranch && wPTool.currentSelect.isBranch))
			{
				if (GUILayout.Button("Create Crossing"))
				{
					wPTool.CreateCrossing();
				}
			}
		}

		if (wPTool.currentSelect != null && wPTool.currentSelect.isBranch && wPTool.currentSelect.nextWaypoint != null)
		{
			if (GUILayout.Button("Remove Crossing Connection"))
			{
				wPTool.RemoveCrossing();
			}
		}
	}

}
