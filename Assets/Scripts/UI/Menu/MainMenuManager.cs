using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.UI.Menu
{
	public class MainMenuManager : MonoBehaviour
	{
		private Transform _lowerBoundary;
		private Transform _upperBoundary;

		public MainMenuItem[] _buttons;

		void Awake()
		{
			_lowerBoundary = this.transform.FindChild("Lower_Boundary");
			_upperBoundary = this.transform.FindChild("Upper_Boundary");

			_buttons = GameObject.FindObjectsOfType<MainMenuItem>();
		}

		public void Click(int _index)
		{
			_buttons[_index].Animate(_lowerBoundary, _upperBoundary);
		}
	}
}
#endregion