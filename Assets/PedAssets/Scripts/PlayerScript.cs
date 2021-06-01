using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	//[SerializeField] GameObject innerCollider;
	//[SerializeField] GameObject outerCollider;

	void Update () 
	{
		transform.Translate(new Vector3(Input.GetAxis("Horizontal") * 5 * Time.deltaTime, 0, Input.GetAxis("Vertical") * 5 * Time.deltaTime));

		Vector3 rot = new Vector3(0, Input.GetAxis("Mouse X") * 50 * Time.deltaTime, 0);
		transform.eulerAngles = transform.eulerAngles + rot;
	}


}
