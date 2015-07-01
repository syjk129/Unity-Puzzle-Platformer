using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

public class BubbleBlowing : MonoBehaviour {
	

	public float start_bubble_distance = 1.0f;
	public float current_bubble_distance = 1.0f;
	public float bubble_height = 1.7f;

	public float max_scale = 3f; //how big can the bubble get
	public float growth_rate = 1.0f; //how fast does the bubble grow
	public float current_scale = 0.1f; //how big is the bubble right now
	public float start_scale = 0.1f; // how big does the bubble start out
	public Texture2D bubble_texture;
	public float bubble_force = 1000;

	private GameObject bubble;
	private PlayerMove playerMoveScript;
	private bool shooting;
	//private Transform mainCam;
	public static int number_of_bubbles = 5;
	private GameObject mainCam;
	private CameraFollow cameraFollowScript;

	// Use this for initialization
	void Start () {
		playerMoveScript = GetComponent<PlayerMove>();
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		cameraFollowScript = mainCam.GetComponent<CameraFollow>();
	}

	void Update () {

		//First time i right click
		if (Input.GetButtonDown ("RB") && playerMoveScript.grounded && number_of_bubbles > 0){
			ToggleBlowing ();
		}
		

		//If im still holding down the button, grow the bubble
		if(shooting){
			if(bubble != null && current_scale < max_scale){
				bubble.transform.localScale = Vector3.one * current_scale;
				current_scale += growth_rate * Time.deltaTime;
				current_bubble_distance += growth_rate * Time.deltaTime; 

			    Vector3 currentLoc = this.transform.position + this.transform.forward * current_bubble_distance *0.5f;
				currentLoc.y += bubble_height + current_bubble_distance * 0.1f;
				bubble.transform.position = currentLoc;
			}
		}

		if (Input.GetButtonDown ("LB") && shooting){
			//makes it so the bubble has to be max size
			if(current_scale >= max_scale){
				print ("FIRE");
				shooting = false;
				playerMoveScript.lockedMovement = false;
				cameraFollowScript.aiming = false;
				FireBubble();
				ResetBubbleBlowing();
				number_of_bubbles--;
				print ("Number of bubbles left: " +number_of_bubbles);
			}
		}
	}
		


	void ToggleBlowing(){
		shooting = !shooting;
		if(shooting){
			print ("Shooting: ON");
			playerMoveScript.lockedMovement = true;
			cameraFollowScript.aiming = true;
			CreateNewBubble();
		}
		else{
			print ("Shooting: OFF");
			Destroy(bubble);
			playerMoveScript.lockedMovement = false;
			cameraFollowScript.aiming = false;
			ResetBubbleBlowing();
		}
	}



	//Chris: creates new bubble new the players location
	void CreateNewBubble(){
		//Vector3 pos = this.transform.position;

		bubble = GameObject.Instantiate(Resources.Load("Bubble")) as GameObject;
		bubble.GetComponent<Bounce>().collisionEnter = false;
		bubble.transform.parent = this.transform;

		bubble.transform.position = this.transform.position + this.transform.forward * start_bubble_distance;
		bubble.transform.rotation = this.transform.rotation;
		Vector3 currentLoc = bubble.transform.position;
		currentLoc.y += bubble_height;
		
		bubble.transform.position = currentLoc;
		bubble.transform.localScale = Vector3.one;
	}


	void FireBubble(){
		if(bubble == null){
			ResetBubbleBlowing();
		}
		Rigidbody bubble_body = bubble.AddComponent<Rigidbody>();
		bubble_body.AddForce(mainCam.transform.forward * bubble_force);
		bubble.GetComponent<Bounce>().collisionEnter = true;
	}


	//Chris: resets parameters so a new bubble can be blown
	void ResetBubbleBlowing(){
		if(bubble != null){
			current_scale = start_scale;
			current_bubble_distance = start_bubble_distance;
			bubble.transform.parent = this.transform.parent;
			bubble = null;
		}
	}

}
