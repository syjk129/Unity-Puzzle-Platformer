using UnityEngine;
using System.Collections;

//add this class to hazards such as lava or spikes, use "effectedTags" to choose which objects can be hurt by this hazard
[RequireComponent(typeof(AudioSource))]
public class Bounce : MonoBehaviour 
{
	public float pushForce = 25f;							//how far away from this object to push the bounceObject when they hit this hazard
	public float pushHeight = 6f;							//how high to push bounceObject when they are hit by this hazard
	public bool triggerEnter;								//are we checking for a trigger collision? (ie: hits a child trigger symbolising area of effect)
	public bool collisionEnter = true;						//are we checking for collider collision? (ie: hits the actual collider of the object)
	public string[] effectedTags = {"Player"};				//which objects are vulnerable to this hazard (tags)
	public AudioClip hitSound;								//sound to play when an object is hurt by this hazard
	
	//setup
	void Awake()
	{
		GetComponent<AudioSource>().playOnAwake = false;
	}
	
	//if were checking for a physical collision, attack what hits this object
	void OnCollisionEnter(Collision col)
	{
		if(!collisionEnter)
			return;
		foreach(string tag in effectedTags)
			if(col.transform.tag == tag)
		{
			MakeBounce (col.gameObject, pushHeight, pushForce);
			if (hitSound)
			{
				GetComponent<AudioSource>().clip = hitSound;
				GetComponent<AudioSource>().Play();
			}
		}
	}
	
	//if were checking for a trigger enter, attack what enters the trigger
	void OnTriggerEnter(Collider other)
	{
		if(!triggerEnter)
			return;
		foreach(string tag in effectedTags)
			if(other.transform.tag == tag)
				MakeBounce (other.gameObject, pushHeight, pushForce);
	}

	public void MakeBounce(GameObject bounceObject, float pushHeight, float pushForce)
	{
		//push
		Vector3 pushDir = (bounceObject.transform.position - transform.position);
		pushDir.y = 0f;
		pushDir.y = pushHeight * 0.1f;
		if (bounceObject.GetComponent<Rigidbody>() && !bounceObject.GetComponent<Rigidbody>().isKinematic)
		{
			bounceObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
			bounceObject.GetComponent<Rigidbody>().AddForce (pushDir.normalized * pushForce, ForceMode.VelocityChange);
			bounceObject.GetComponent<Rigidbody>().AddForce (Vector3.up * pushHeight, ForceMode.VelocityChange);
		}
	}
}

/* NOTE: a nice feature of unity is that the trigger enter check works with a child object trigger
 * so you might have a physical collider on the actual object, then a child trigger for the damage area
 * for example: a lawnmower which the player can stand on, and a blade on the front which damages objects */