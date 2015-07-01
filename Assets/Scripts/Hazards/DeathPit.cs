using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Hazards
{
	public class DeathPit : MonoBehaviour
	{
		private bool _isColliding;

		void OnTriggerEnter(Collider _col)
		{
			if(_col.tag.Equals("Player"))
			{
				if(_isColliding) return;
				_isColliding = true;
				_col.GetComponent<Player.PlayerLife>().Health = 0f;
			}
		}
	}
}
#endregion