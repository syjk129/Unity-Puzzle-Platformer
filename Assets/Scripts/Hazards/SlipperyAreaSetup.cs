using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Hazards
{
	public class SlipperyAreaSetup : MonoBehaviour
	{
		private Transform _point1;
		private Transform _point2;

		private Transform _colliderObject;

		public GameObject _beg;
		public GameObject _mid;
		public GameObject _end;
		public Canvas _worldCanvas;

		private float _waterSegmentLength = 0.5f;

		void Start()
		{
			_point1 = this.transform.FindChild("Point_1");
			_point2 = this.transform.FindChild("Point_2");

			//_point1.LookAt(_point2);

			_colliderObject = this.transform.FindChild("Slip_Trigger");

			RaycastHit _hit;
			if(Physics.Raycast(_point1.position, Vector3.down,out _hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Ground"))))
			{
				_point1.position = _hit.point;
			}
			else
			{
				Debug.LogError("Point 1 is not above ground!");
			}
			if(Physics.Raycast(_point2.position, Vector3.down, out _hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Ground"))))
			{
				_point2.position = _hit.point;

				Vector3 relativePos = _point2.position - _point1.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos, _hit.normal);
				_point2.rotation = rotation;
				_point1.rotation = rotation;
			}
			else
			{
				Debug.LogError("Point 2 is not above ground!");
			}

			float _size = Mathf.Abs(Vector3.Distance(_point1.position, _point2.position));
			Vector3 _midPoint = (_point2.position + _point1.position) * 0.5f;

			_colliderObject.position = _midPoint;
			_colliderObject.rotation = _point2.rotation;
			_colliderObject.localScale = new Vector3(0.5f, 0.2f, _size);

			float _steps = _size/_waterSegmentLength;

			for(int i = 0; i < _steps; i++)
			{
				if(i > 0)
				{
					GameObject _newSegment;

					Vector3 _nextPoint = (_point2.position - _point1.position) * (i/_steps);
					_nextPoint += _point1.position;

					if(i >= _steps - 1) _newSegment = (GameObject)Instantiate(_end, _nextPoint, _point2.rotation);
					else _newSegment = (GameObject)Instantiate(_mid, _nextPoint, _point1.rotation);

					_newSegment.transform.parent = _worldCanvas.transform;
				}
				else
				{
					GameObject _newBeginning = (GameObject)Instantiate(_beg, _point1.position, _point1.rotation);
					_newBeginning.transform.parent = _worldCanvas.transform;
				}
			}
		}
	}
}
#endregion