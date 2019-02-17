using UnityEngine;
using System.Collections;

// FastProjectiles test v1.0 (projectileMover)
// http://unitycoder.com/blog

public class ProjectileMover : MonoBehaviour 
{
	public float movespeed;
	public float distanceToTravel;
	
	void FixedUpdate () 
	{
		if (distanceToTravel<=0.001f) // have we arrived our target?
		{
			// we arrived target, lets destroy this script, then destroy the gameObject after 10secs
			Destroy(this);
			Destroy(gameObject,10);
		}else{
			transform.Translate(Vector3.forward * movespeed);
			distanceToTravel-=movespeed;
		}
	}
}
