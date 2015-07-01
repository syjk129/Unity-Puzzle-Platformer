using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Data
{
	public class GardenSwitcher : MonoBehaviour
	{
		public string[] _gardens;

		private static GardenSwitcher _instance;
		
		void Awake()
		{
			if(_instance == null)
			{
				DontDestroyOnLoad(this.gameObject);
				_instance = this;
			}
			else if(_instance != this)
			{
				Debug.Log("Too many garden switchers");
				Destroy(this.gameObject);
			}
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				Application.LoadLevel(_gardens[0]);
				BubbleBlowing.number_of_bubbles = 5;
			}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				Application.LoadLevel(_gardens[1]);
				BubbleBlowing.number_of_bubbles = 5;
			}
			if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				Application.LoadLevel(_gardens[2]);
				BubbleBlowing.number_of_bubbles = 5;
			}
			if(Input.GetKeyDown(KeyCode.Alpha4))
			{
				Application.LoadLevel(_gardens[3]);
				BubbleBlowing.number_of_bubbles = 5;
			}
			if(Input.GetKeyDown(KeyCode.Alpha5))
			{
				Application.LoadLevel(_gardens[4]);
				BubbleBlowing.number_of_bubbles = 5;
			}
		}
	}
}
#endregion