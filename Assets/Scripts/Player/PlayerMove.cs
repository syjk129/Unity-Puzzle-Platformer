using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player
{
	//handles player movement, utilising the CharacterMotor class
	[RequireComponent(typeof(CharacterMotor))]
	[RequireComponent(typeof(AudioSource))]
	public class PlayerMove : PlayerControllerObject 
	{
		//setup
		public Transform mainCam, floorChecks;		//main camera, and floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
		public Animator animator;					//object with animation controller on, which you want to animate
		public AudioClip jumpSound;					//play when jumping
		public AudioClip landSound;					//play when landing on ground
		public AudioClip grassSound;
		
		//movement
		public float accel = 70f;					//acceleration/deceleration in air or on the ground
		public float airAccel = 18f;			
		public float decel = 7.6f;
		public float airDecel = 1.1f;
		[Range(0f, 5f)]
		public float rotateSpeed = 0.7f, airRotateSpeed = 0.4f;	//how fast to rotate on the ground, how fast to rotate in the air
		public float maxSpeed = 9;								//maximum speed of movement in X/Z axis
		public float slopeLimit = 40, slideAmount = 35;			//maximum angle of slopes you can walk on, how fast to slide down slopes you can't
		public float movingPlatformFriction = 7.7f;				//you'll need to tweak this to get the player to stay on moving platforms properly
		
		//jumping
		public Vector3 jumpForce =  new Vector3(0, 13, 0);		//normal jump force
		public float jumpDelay = 0.1f;							//how fast you need to jump after hitting the ground, to do the next type of jump
		public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
		[HideInInspector]
		public int onEnemyBounce;					
		public bool lockedMovement = false;
		
		private int onJump;
		public bool grounded;
		private Transform[] floorCheckers;
		private Quaternion screenMovementSpace;
		private float airPressTime, groundedCount, curAccel, curDecel, curRotateSpeed, slope;
		private Vector3 direction, moveDirection, screenMovementForward, screenMovementRight, movingObjSpeed;
		
		private CharacterMotor characterMotor;
		//private Bounce bounce;
		public bool isMoving;
		
		#region SABRINA
		
		public bool isCrawling = false;
		[HideInInspector]
		public BoxCollider box;
		public BoxCollider crawlUnder;
		
		#endregion
		
		#region ERIC
		
		static int _slipState = Animator.StringToHash("Base Layer.Slip");
		private AnimatorStateInfo _currentBaseState;
		
		private Vector3 _slipVector;
		private float _slipSpeed = 10f;
		
		public GameObject _ragdoll;
		public GameObject _model;
		
		//setting up events
		void OnEnable()
		{
			PlayerLife.PlayerDie += EnableRagdoll;
		}
		
		void OnDisable()
		{
			PlayerLife.PlayerDie -= EnableRagdoll;
		}
		
		public void EnableRagdoll()
		{
			this.GetComponent<Rigidbody>().isKinematic = true;
			animator.enabled = false;
			this.GetComponent<Collider>().enabled = false;
			
			_ragdoll.transform.position = this.transform.position;
			_ragdoll.transform.rotation = this.transform.rotation;
			_model.gameObject.SetActive(false);
			_ragdoll.gameObject.SetActive(true);
		}
		
		public void Slip(Vector3 _direction)
		{
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			animator.SetBool("Slip", true);
			_slipVector = _direction * _slipSpeed;
			animator.speed = 1.5f;
		}
		
		public void StopSlipping()
		{
			animator.SetBool("Slip", false);
			animator.speed = 1.0f;
			//_slipVector = Vector3.zero;
		}
		#endregion
		
		//setup
		void Awake()
		{
			box = GetComponent<BoxCollider> ();
			
			//animator = GetComponent <Animator> ();
			//create single floorcheck in centre of object, if none are assigned
			if(!floorChecks)
			{
				floorChecks = new GameObject().transform;
				floorChecks.name = "FloorChecks";
				floorChecks.parent = transform;
				floorChecks.position = transform.position;
				GameObject check = new GameObject();
				check.name = "Check1";
				check.transform.parent = floorChecks;
				check.transform.position = transform.position;
				Debug.LogWarning("No 'floorChecks' assigned to PlayerMove script, so a single floorcheck has been created", floorChecks);
			}
			//assign player tag if not already
			if(tag != "Player")
			{
				tag = "Player";
				Debug.LogWarning ("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
			}
			//usual setup
			mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
			//bounce = GetComponent<Bounce>();
			characterMotor = GetComponent<CharacterMotor>();
			//gets child objects of floorcheckers, and puts them in an array
			//later these are used to raycast downward and see if we are on the ground
			floorCheckers = new Transform[floorChecks.childCount];
			for (int i=0; i < floorCheckers.Length; i++)
				floorCheckers[i] = floorChecks.GetChild(i);
		}
		
		//get state of player, values and input
		public override void Run()
		{	
			_currentBaseState = animator.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
			//handle jumping
			JumpCalculations ();
			//adjust movement values if we're in the air or on the ground
			curAccel = (grounded) ? accel : airAccel;
			curDecel = (grounded) ? decel : airDecel;
			curRotateSpeed = (grounded) ? rotateSpeed : airRotateSpeed;
			
			//get movement axis relative to camera
			screenMovementSpace = Quaternion.Euler (0, mainCam.eulerAngles.y, 0);
			screenMovementForward = screenMovementSpace * Vector3.forward;
			screenMovementRight = screenMovementSpace * Vector3.right;
			
			//get movement input, set direction to move in
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");

			//crawl with the key "c" pressed down
			
			if (Input.GetKey (KeyCode.C)) {
				Crawl ();
			} else {
				StopCrawl();
			}


			//only apply vertical input to movemement, if player is not sidescroller
			
			direction = (screenMovementForward * v) + (screenMovementRight * h);
			moveDirection = transform.position + direction;
			
			//SHAWN 
			if (h != 0 || v != 0) {
				isMoving = true;
			} else {
				isMoving = false;
			}
		}
		
		//apply correct player movement (fixedUpdate for physics calculations)
		public override void FixedRun()
		{
			//are we grounded
			grounded = IsGrounded ();
			Vector3 lookDirection = transform.position + mainCam.forward;
			
			#region ERIC
			
			if(_currentBaseState.fullPathHash == _slipState)
			{
				_slipVector = new Vector3(_slipVector.x, this.GetComponent<Rigidbody>().velocity.y, _slipVector.z);
				this.GetComponent<Rigidbody>().velocity = _slipVector;
			}
			#endregion
			if(_currentBaseState.fullPathHash != _slipState)
			{
				//Chris: added ability to lock player movement to just rotation (for shooting bubbles)
				if(lockedMovement && grounded){
					characterMotor.RotateToDirection (lookDirection, curRotateSpeed * 20, true);
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					animator.SetFloat("Speed", 0f);
				}
				else{
					//move, rotate, manage speed
					characterMotor.customMaxSpeed = maxSpeed + movingObjSpeed.magnitude;
					if (rotateSpeed != 0 && direction.magnitude != 0)
						characterMotor.RotateToDirection (moveDirection , curRotateSpeed * 5, true);
					characterMotor.MoveTo (moveDirection, curAccel, 0.7f, true);
					characterMotor.ManageSpeed (curDecel, maxSpeed + movingObjSpeed.magnitude, true);
					
					
					//set animation values
					if(animator)
					{
						animator.SetBool("Grounded", grounded);
						animator.SetFloat("YVelocity", GetComponent<Rigidbody>().velocity.y);
						float speed = Mathf.Abs (Input.GetAxisRaw("Horizontal")) + Mathf.Abs (Input.GetAxisRaw ("Vertical"));
						animator.SetFloat("Speed", speed);
						animator.SetBool("Crawling",isCrawling);
					}
				}
			}
		}
		
		//prevents rigidbody from sliding down slight slopes (read notes in characterMotor class for more info on friction)
		void OnCollisionStay(Collision other)
		{
			//only stop movement on slight slopes if we aren't being touched by anything else
			if (other.collider.tag != "Untagged" || grounded == false)
				return;
			//if no movement should be happening, stop player moving in Z/X axis
			if(direction.magnitude == 0 && slope < slopeLimit && GetComponent<Rigidbody>().velocity.magnitude < 2)
			{
				//it's usually not a good idea to alter a rigidbodies velocity every frame
				//but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it shou
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
		
		//returns whether we are on the ground or not
		//also: bouncing on enemies, keeping player on moving platforms and slope checking
		private bool IsGrounded() 
		{
			//get distance to ground, from centre of collider (where floorcheckers should be)
			float dist = GetComponent<Collider>().bounds.extents.y;
			//check whats at players feet, at each floorcheckers position
			foreach (Transform check in floorCheckers)
			{
				RaycastHit hit;
				if(Physics.Raycast(check.position, Vector3.down, out hit, dist + 0.05f))
				{
					if(!hit.transform.GetComponent<Collider>().isTrigger)
					{
						//slope control
						slope = Vector3.Angle (hit.normal, Vector3.up);
						//slide down slopes
						if(slope > slopeLimit && hit.transform.tag != "Pushable")
						{
							Vector3 slide = new Vector3(0f, -slideAmount, 0f);
							GetComponent<Rigidbody>().AddForce (slide, ForceMode.Force);
						}
						//moving platforms
						if (hit.transform.tag == "MovingPlatform" || hit.transform.tag == "Pushable")
						{
							movingObjSpeed = hit.transform.GetComponent<Rigidbody>().velocity;
							movingObjSpeed.y = 0f;
							//9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
							GetComponent<Rigidbody>().AddForce(movingObjSpeed * movingPlatformFriction * Time.fixedDeltaTime, ForceMode.VelocityChange);
						}
						else
						{
							movingObjSpeed = Vector3.zero;
						}
						//yes our feet are on something
						return true;
					}
				}
			}
			movingObjSpeed = Vector3.zero;
			//no none of the floorchecks hit anything, we must be in the air (or water)
			return false;
		}
		
		//jumping
		private void JumpCalculations()
		{
			//keep how long we have been on the ground
			groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;
			
			//play landing sound
			if(groundedCount < 0.25 && groundedCount != 0 && !GetComponent<AudioSource>().isPlaying && landSound && GetComponent<Rigidbody>().velocity.y < 1)
			{
				GetComponent<AudioSource>().volume = Mathf.Abs(GetComponent<Rigidbody>().velocity.y)/40;
				GetComponent<AudioSource>().clip = landSound;
				GetComponent<AudioSource>().Play ();
			}
			//if we press jump in the air, save the time
			if (Input.GetButtonDown ("Jump") && !grounded)
				airPressTime = Time.time;
			
			//if were on ground within slope limit
			if (grounded && slope < slopeLimit)
			{
				//and we press jump, or we pressed jump justt before hitting the ground
				if (Input.GetButtonDown ("Jump") || airPressTime + jumpLeniancy > Time.time)
				{	
					//increment our jump type if we haven't been on the ground for long
					onJump = (groundedCount < jumpDelay) ? Mathf.Min(2, onJump + 1) : 0;
					//execute the correct jump (like in mario64, jumping 3 times quickly will do higher jumps)
					if (onJump == 0)
					{
						Jump (jumpForce);
						StopSlipping();
					}
				}
			}
		}
		
		//push player at jump force
		public void Jump(Vector3 jumpVelocity)
		{
			if(!lockedMovement){
				if(jumpSound)
				{
					GetComponent<AudioSource>().clip = jumpSound;
					Data.SoundManager.PlaySFX(this.GetComponent<AudioSource>());
				}
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z);
				GetComponent<Rigidbody>().AddRelativeForce (jumpVelocity, ForceMode.Impulse);
				airPressTime = 0f;
			}
		}
		
		#region SABRINA
		public void Crawl() {
			maxSpeed = 3.0f;
			box.size = new Vector3 (box.size.x, 1.0f, box.size.z);
			box.center = new Vector3 (box.center.x, 0.5f, box.center.z);
			isCrawling = true;
			
			
		}
		
		public void StopCrawl() {
			//Check if she is colliding when she stands up
			box.size = new Vector3 (box.size.x, 2.5f, box.size.z);
			box.center = new Vector3 (box.center.x, 1.25f, box.center.z);
			bool isStuck = false;
			//Collider[] colliders = FindObjectsOfType (typeof(Collider)) as Collider[];
			/*foreach (Collider checkBox in colliders) {
			if (checkBox != box && !checkBox.isTrigger) {
				isStuck = box.bounds.Intersects (checkBox.bounds);
				if (isStuck) {
					print (checkBox);
					break;
				}
			}
		}*/
			
			if (crawlUnder != null) {
				isStuck = box.bounds.Intersects (crawlUnder.bounds);
			}
			if (!isStuck) {
				maxSpeed = 9.0f;
				isCrawling = false;
			} else {	
				box.size = new Vector3 (box.size.x, 1.0f, box.size.z);
				box.center = new Vector3 (box.center.x, 0.5f, box.center.z);
			}
		}
		
		#endregion
		
	}
}