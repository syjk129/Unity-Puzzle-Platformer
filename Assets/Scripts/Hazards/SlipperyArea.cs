using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Hazards
{
	public class SlipperyArea : MonoBehaviour
	{

		private bool _isColliding;

		void OnTriggerEnter(Collider _col)
		{
			if(_isColliding) return;
			_isColliding = true;
			if(_col.tag.Equals("Player")) _col.GetComponent<Player.PlayerMove>().Slip(this.transform.forward);
			else if(_col.tag.Equals("Pickup")) _col.GetComponent<Player.ObjectSlider>().Slide(this.transform.forward);
		}

		void OnTriggerExit(Collider _col)
		{
			if(_col.tag.Equals("Player")) _col.GetComponent<Player.PlayerMove>().StopSlipping();
			else if(_col.tag.Equals("Pickup")) _col.GetComponent<Player.ObjectSlider>().StopSliding();
			_isColliding = false;
		}
	}
}
#endregion