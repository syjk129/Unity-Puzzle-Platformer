using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Player
{
	public class ObjectSlider : MonoBehaviour
	{
		private Rigidbody _rigidbody;
		private bool _sliding = false;
		private Vector3 _direction;
		private float _slideSpeed = 10f;

		void Awake()
		{
			_rigidbody = this.GetComponent<Rigidbody>();
		}

		void FixedUpdate()
		{
			if(_sliding)
			{
				_rigidbody.MovePosition(this.transform.position + _direction * Time.deltaTime * _slideSpeed);
			}
		}

		public void Slide(Vector3 _direction)
		{
			this._direction = _direction;
			_sliding = true;
		}

		public void StopSliding()
		{
			_sliding = false;
		}
	}
}
#endregion