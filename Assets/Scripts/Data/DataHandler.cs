using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#region ERIC
namespace Assets.Scripts.Data
{
	public abstract class DataHandler : MonoBehaviour
	{
		protected static string _audioDataPath = Application.persistentDataPath + "/Audio.dat";

		void Awake()
		{
			Init();
		}

		protected abstract void Init();
	}

	[Serializable]
	public class AudioData
	{
		private float _sfxVol;
		private float _musicVol;

		public AudioData()
		{
			_sfxVol = 1f;
			_musicVol = 1f;
		}
		
		public float SFXVol
		{
			get { return _sfxVol; }
			set { _sfxVol = value; }
		}
		public float MusicVol
		{
			get { return _musicVol; }
			set { _musicVol = value; }
		}
	}
}
#endregion