using UnityEngine;
using System.Collections;
#region ERIC
namespace Assets.Scripts.Util
{
	public class AnimationInfo : MonoBehaviour
	{
		private float _speed = 0f;

		void OnEnable()
		{
			Data.GameManager.GamePause += PauseAnimator;
			Data.GameManager.GameUnpause += UnpauseAnimator;
		}
		void OnDisable()
		{
			Data.GameManager.GamePause -= PauseAnimator;
			Data.GameManager.GameUnpause -= UnpauseAnimator;
		}
		
		public void PauseAnimator()
		{
			_speed = this.GetComponent<Animator>().speed;
			this.GetComponent<Animator>().speed = 0;
		}
		
		public void UnpauseAnimator()
		{
			this.GetComponent<Animator>().speed = _speed;
		}
	}
}
#endregion