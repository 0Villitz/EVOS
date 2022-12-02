using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour {
	[SerializeField]Transform spawnPoint;


	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.transform.CompareTag("Player"))
			col.transform.position = spawnPoint.position;
	}
}
