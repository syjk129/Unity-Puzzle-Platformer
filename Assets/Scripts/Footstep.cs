using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;
using Assets.Scripts.Data;
//by: Shawn Kim
public class Footstep : MonoBehaviour {

	PlayerMove cc;
	new AudioSource audio;

	void Start () {
		cc = GetComponent<PlayerMove> ();
		audio = GetComponent<AudioSource>();
	}

	void Update () {
		//Temp for grass sound only
		if(cc.grounded == true && cc.lockedMovement == false && cc.isMoving){
			audio.clip = cc.grassSound;
			//audio.volume = Random.Range (0.8f, 1);
			audio.pitch = 2f;
			SoundManager.PlaySFX(audio);
		}
	}
}
