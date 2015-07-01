using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Player
{
	public class PushPull : PlayerControllerObject
	{
		Animator anim;							
		Rigidbody body2;
		FixedJoint fj;
		Rigidbody self;
		//BoxCollider col;
		float h, v;
		int flag = 0;

		void Start()
		{
			anim = GetComponentInChildren<Animator>();					  			
			self = GetComponent<Rigidbody>();
			//col = GetComponent<BoxCollider>();
		}

		public override void Run ()
		{
			throw new System.NotImplementedException ();
		}

		public override void FixedRun () 
		{
			h = Input.GetAxis("Horizontal");				
			v = Input.GetAxis("Vertical");				
			anim.SetBool("Push",false);
			//anim.SetBool("Pull",false);

			if(body2!=null && !Input.GetKey(KeyCode.LeftShift))
			{
				body2=null;
			}
			//Vector3 move = new Vector3(h,0,v);
			//float dotProd = Vector3.Dot(move, transform.forward);

			if(flag ==1 && Input.GetKey(KeyCode.LeftShift) && (h!=0 || v!=0) && (h*v==0) && body2!=null)
			{
				if((Input.GetKey("up") && Input.GetKey("down")) || (Input.GetKey("right") && Input.GetKey("left")))
				{
					anim.SetBool("Push",false);
				}
				else
				{
					anim.SetBool("Push",true);
					body2.velocity = self.transform.forward*4;
				}
			}
			else 
			{
				if(body2!=null)
				{
					body2=null;
				}
				flag = 0;
				if(fj!=null)
				{
					Component.Destroy(fj);
				}
			}
		}
		void OnCollisionStay(Collision collision) 
		{
			if(collision.collider.attachedRigidbody != null && Input.GetKey(KeyCode.LeftShift) && collision.collider.attachedRigidbody.tag == "grabbable")
			{
				body2 = collision.collider.attachedRigidbody;
				flag=1;
				if(fj==null)
				{
					gameObject.AddComponent<FixedJoint>();
					fj = GetComponent<FixedJoint>();
					fj.connectedBody = body2;
				}	
			}
			else if(fj!=null && !Input.GetKey(KeyCode.LeftShift))
			{
				Component.Destroy(fj);
			}
		}
	}
}
