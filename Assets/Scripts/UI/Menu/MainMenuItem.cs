using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.UI.Menu
{
	public class MainMenuItem : MonoBehaviour
	{
		private Transform _lowerBoundary;
		private Transform _upperBoundary;
		private bool _moveUp = false;
		private float _speed = 0.3f;
		private float _gapDistance = 2f;

		private float _vel = 0f;

		void Update()
		{
			if(_lowerBoundary && !_moveUp)
			{
				float _y = Mathf.SmoothDamp(this.transform.position.y, _lowerBoundary.transform.position.y, ref _vel, _speed);
				this.transform.position = new Vector3(this.transform.position.x, _y, this.transform.position.z);
				if(_y - _lowerBoundary.position.y < _gapDistance)
				{
					transform.position = _lowerBoundary.position;
					_moveUp = true;
				}
			}
			else if(_upperBoundary && _moveUp)
			{
				float _y = Mathf.SmoothDamp(this.transform.position.y, _upperBoundary.transform.position.y, ref _vel, _speed);
				this.transform.position = new Vector3(this.transform.position.x, _y, this.transform.position.z);
				if(_upperBoundary.position.y - _y < _gapDistance)
				{
					transform.position = _upperBoundary.position;
					_moveUp = false;
					_lowerBoundary = null;
					_upperBoundary = null;
				}
			}
		}

		public void Animate(Transform _lowerBoundary, Transform _upperBoundary)
		{
			this._lowerBoundary = _lowerBoundary;
			this._upperBoundary = _upperBoundary;
		}
	}
}
#endregion