using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#region ERIC
namespace Assets.Scripts.Data
{
	public class SaveManager : DataHandler
	{
		private static SaveManager _instance;

		protected override void Init()
		{
			if(_instance == null)
			{
				DontDestroyOnLoad(this.gameObject);
				_instance = this;
			}
			else if(_instance != this)
			{
				Debug.Log("Too many save managers");
				Destroy(this.gameObject);
			}
		}

		public static void SaveAudio(float _sfxVol, float _musicVol)
		{

			BinaryFormatter _bf = new BinaryFormatter();
			FileStream _file = File.Create(_audioDataPath);

			AudioData _data = new AudioData();
			_data.SFXVol = _sfxVol;
			_data.MusicVol = _musicVol;

			_bf.Serialize(_file, _data);
			_file.Close();
		}
	}
}
#endregion