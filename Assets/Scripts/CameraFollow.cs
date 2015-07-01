using UnityEngine;
using Assets.Scripts.Player;

public class CameraFollow : MonoBehaviour 
{
	public Transform target;									//object camera will focus on and follow
	public Vector3 targetOffset;								//how far back should camera be from the lookTarget
	public Vector3 followOffset =  new Vector3(0f, 2.39f, 4.35f);	//how far back should camera be from the lookTarget
	public Vector3 aimOffset = new Vector3(0f, 2f, 2f);

	public float followSpeed = 6;								//how fast the camera moves to its intended position
	public float inputRotationSpeed = 300;						//how fast the camera rotates around lookTarget when you press the camera adjust buttons
	public float rotateDamping = 100;							//how fast camera rotates to look at target
	public string[] avoidClippingTags;							//tags for big objects in your game, which you want to camera to try and avoid clipping with
	public float freelookHeight = 20f;
	private Transform followTarget;

	private bool camColliding;
	public bool mouseFreelook = false;									//should the camera be rotated with the mouse? (only if camera is not fixed)
	public bool lockRotation;									//should the camera be fixed at the offset (for example: following behind the player)
	public bool aiming = false;

	#region ERIC

	public GameObject _ragdoll;

	//setting up events
	void OnEnable()
	{
		PlayerLife.PlayerDie += WatchRagdoll;
	}
	
	void OnDisable()
	{
		PlayerLife.PlayerDie -= WatchRagdoll;
	}

	public void WatchRagdoll()
	{
		target = _ragdoll.transform;
	}
	#endregion

	//setup objects
	void Awake()
	{
		followTarget = new GameObject().transform;	//create empty gameObject as camera target, this will follow and rotate around the player
		followTarget.name = "Camera Target";
		targetOffset = followOffset;
		if(!target)
			Debug.LogError("'CameraFollow script' has no target assigned to it", transform);
		
		//don't smooth rotate if were using mouselook
		if(mouseFreelook)
			rotateDamping = 0f;
	}
	
	//run our camera functions each frame
	void Update()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		if (!target)
			return;

		SmoothFollow ();
		if(rotateDamping > 0)
			SmoothLookAt();
		else
			transform.LookAt(target.position);
	}
	
	//rotate smoothly toward the target
	void SmoothLookAt()
	{
		Quaternion rotation = Quaternion.LookRotation (target.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, rotateDamping * Time.deltaTime);
	}
		
	//move camera smoothly toward its target
	void SmoothFollow()
	{
		//move the followTarget (empty gameobject created in awake) to correct position each frame
		followTarget.position = target.position;

		//Chris: set the position of the camera based on if you're aiming or not
		if(aiming)
			targetOffset = aimOffset;
		else 
			targetOffset = followOffset;

		followTarget.Translate(targetOffset, Space.Self);
		if (lockRotation)
			followTarget.rotation = target.rotation;
		float axisX = Input.GetAxis ("Mouse X") * inputRotationSpeed * Time.deltaTime;
		followTarget.RotateAround (target.position, Vector3.up, axisX);
		float axisY = Input.GetAxis ("Mouse Y") * inputRotationSpeed * Time.deltaTime;
		followTarget.RotateAround (target.position, transform.right, -axisY);
		
		//where should the camera be next frame?
		Vector3 nextFramePosition = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
		Vector3 direction = nextFramePosition - target.position;
		//raycast to this position
		RaycastHit hit;
		if(Physics.Raycast (target.position, direction, out hit, direction.magnitude + 0.3f))
		{
			transform.position = nextFramePosition;
			foreach(string tag in avoidClippingTags)
				if(hit.transform.tag == tag)
					transform.position = hit.point - direction.normalized * 0.3f;
		}
		else
		{
			//otherwise, move cam to intended position
			transform.position = nextFramePosition;
		}
	}
}