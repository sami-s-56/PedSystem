using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WaypointDisplay 
{
	[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
	public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
	{
		if((gizmoType  & GizmoType.Selected) != 0)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.yellow;
		}

		Gizmos.DrawSphere(waypoint.transform.position, .5f);
		Gizmos.DrawWireSphere(waypoint.transform.position, waypoint.radius);

		Gizmos.color = Color.green;

		if(waypoint.nextWaypoint != null)
		{
			Gizmos.DrawLine(waypoint.transform.position, waypoint.nextWaypoint.transform.position);
		}

		if(waypoint.branches != null)
		{
			foreach (Waypoint branch in waypoint.branches)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
			}
		}

		//Debug.Log("Gettingcalled");
	}
}
