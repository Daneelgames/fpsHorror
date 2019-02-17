using UnityEngine;
using System.Collections;

// FastProjectiles test v1.0 (projectile shooter)
// http://unitycoder.com/blog

public class ShootProjectile : MonoBehaviour 
{

	public Transform prefab;
	public GUIText guispeed;
	public float speed = 20.0f; // speedsteps (ie. how many FixedUpdates() it takes to reach target
	
	void Update () 
	{
		
		if (Input.GetMouseButtonDown(0)) // left button clicked
		{
			// do raycast to see where we clicked
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100))
			{
				// get direction/rotation, instantiate projectile
				Vector3 dir = hit.point - transform.position;
				Quaternion rot = Quaternion.LookRotation(dir);
				Transform clone = Instantiate(prefab, transform.position, rot) as Transform;
				
				// set projectile variables
				ProjectileMover other;
				other = clone.gameObject.GetComponent("ProjectileMover") as ProjectileMover;
				other.distanceToTravel = Vector3.Distance(transform.position, hit.point);
				other.movespeed = other.distanceToTravel/speed; // this is the movestep that we take each FixedUpdate()
			}
		}
	}

	// gui scrollbar for setting the speed
	void OnGUI() 
	{
		speed = Mathf.Floor(GUI.HorizontalSlider(new Rect(25, 25, 300, 40), speed, 100.0F, 1.0F));
		guispeed.text = "SpeedSteps:"+speed;
	}
}
