using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BoxMove : MonoBehaviour {

	public AudioClip boxMoving;
	AudioSource boxMove;
	float h, v;
	// Use this for initialization
	void Start () {
		boxMove = GetComponent<AudioSource>();
		boxMove.clip = boxMoving;
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxis("Horizontal");				
		v = Input.GetAxis("Vertical");	
	
	}
	void OnCollisionStay(Collision collision)
	{
		if(!boxMove.isPlaying && collision.collider.attachedRigidbody!=null && Input.GetKey(KeyCode.LeftShift) && (h!=0 || v!=0))
			boxMove.Play();
		else if(boxMove.isPlaying && !(Input.GetKey(KeyCode.LeftShift) && (h!=0 || v!=0)))
			boxMove.Stop ();
	}
}
