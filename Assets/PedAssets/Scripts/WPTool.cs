using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WPTool : MonoBehaviour 
{
	public Transform wpNetworkHolder;
	public Transform waypointRoot;

	public GameObject activeObject;

	public Waypoint previousSelect;
	public Waypoint currentSelect;

	public bool isSubscribed = false;
	public void AddArea()
	{
		GameObject obj = new GameObject("WaypointArea");
		obj.transform.position = new Vector3(0f, 0f, 0f);
		obj.AddComponent<PedSpawner>();
		Selection.activeObject = obj;
	}

	void OnDrawGizmos()
	{
		print("Gizmos!");
	}

	public void AddNewNetwork()
	{
		if (wpNetworkHolder == null)
		{
			AddArea();
		}

		GameObject obj = new GameObject("WaypointNetwork" + wpNetworkHolder.childCount + 1);
		obj.transform.parent = wpNetworkHolder;
		obj.transform.position = new Vector3(0f, 0f, 0f);
		obj.AddComponent<BlockScript>();
		Selection.activeObject = obj;

	}

	public void RemoveCrossing()
	{
		currentSelect.nextWaypoint = null;
	}

	public void CreateCrossing()
	{
		if (previousSelect.isBranch)
		{
			if (previousSelect.nextWaypoint != null)
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

	public void AddBranch()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

		Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
		if (branchedFrom == null)
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

	public void CreateWaypoint()
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

	public void CreateWaypointBefore()
	{
		GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
		waypointObject.transform.SetParent(waypointRoot, false);

		Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
		Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

		newWaypoint.transform.position = selectedWaypoint.transform.position;
		newWaypoint.transform.forward = selectedWaypoint.transform.forward;

		if (selectedWaypoint.previousWaypoint != null)
		{
			newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
			newWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
		}

		selectedWaypoint.previousWaypoint = newWaypoint;
		newWaypoint.nextWaypoint = selectedWaypoint;

		newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

		Selection.activeGameObject = newWaypoint.gameObject;
	}

	public void CreateWaypointAfter()
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

	public void DeleteWaypoint()
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
