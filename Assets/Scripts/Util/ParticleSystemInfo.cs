using UnityEngine;
using System.Collections;
#region ERIC
namespace Assets.Scripts.Util
{
	public class ParticleSystemInfo : MonoBehaviour
	{
		void OnEnable()
		{
			Data.GameManager.GamePause += PauseParticles;
			Data.GameManager.GameUnpause += UnpauseParticles;
		}
		void OnDisable()
		{
			Data.GameManager.GamePause -= PauseParticles;
			Data.GameManager.GameUnpause -= UnpauseParticles;
		}

		public void PauseParticles()
		{
			this.GetComponent<ParticleSystem>().Pause();
		}

		public void UnpauseParticles()
		{
			this.GetComponent<ParticleSystem>().Play();
		}
	}
}
#endregion