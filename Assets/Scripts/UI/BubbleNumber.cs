using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BubbleNumber : MonoBehaviour {


	Text t;
	// Use this for initialization
	void Start () {
	
		t = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		t.text =  BubbleBlowing.number_of_bubbles.ToString();
			//bb.number.ToString();
	}
}
