using UnityEngine;
using System.Collections;

public class SpawnBeaconCollision : MonoBehaviour {

	void OnTriggerEnter(Collider collider)
	{
		//print(collider.transform.tag);
		GetComponentInParent<SpawnBeaconController>().OnTriggerEnterChild(collider.transform.tag);
	}

	void OnTriggerExit(Collider collider)
	{
		//print(collider.transform.tag);
		GetComponentInParent<SpawnBeaconController>().OnTriggerExitChild(collider.transform.tag);
	}
}
