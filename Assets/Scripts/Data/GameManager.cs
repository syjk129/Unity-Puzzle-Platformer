using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Data
{
	public class GameManager : MonoBehaviour
	{
		public enum GameState
		{
			Loading,
			Running,
			Paused,
			Win,
			Lose,
			Dead
		}

		private static GameManager _instance;

		private static GameState _state;

		public delegate void PauseAction();
		public static event PauseAction GamePause;
		public static event PauseAction GameUnpause;

		void Awake()
		{
			if(_instance == null)
			{
				DontDestroyOnLoad(this.gameObject);
				_instance = this;
			}
			else if(_instance != this)
			{
				Debug.Log("Too many game managers");
				Destroy(this.gameObject);
			}

			_state = GameState.Running;
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Return) && !IsPaused)
			{
				_state = GameState.Paused;
				if(GamePause != null) GamePause();
			}
			if(Input.GetKeyDown(KeyCode.Backspace) && IsPaused)
			{
				_state = GameState.Running;
				if(GameUnpause != null) GameUnpause();
			}
		}

		public static void ResetLevel()
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		public static bool IsPaused
		{
			get { return _state.Equals(GameState.Paused); }
		}
		public static bool InSuspendedState
		{
			get { return _state.Equals(GameState.Paused) || _state.Equals(GameState.Win) || _state.Equals(GameState.Lose); }
		}
	}
}
#endregion