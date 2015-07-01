using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Util
{
	public class ParticleDestroyer : MonoBehaviour
	{

		public float _runTime = 5f;
		private float _timer = 0f;

		void Update()
		{
			_timer += Time.deltaTime;
			if(_timer > _runTime) Destroy(this.gameObject);
		}
	}
}
#endregion