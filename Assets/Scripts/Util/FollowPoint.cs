using UnityEngine;
using System.Collections;

#region ERIC
namespace Assets.Scripts.Util
{
	public class FollowPoint : MonoBehaviour
	{
		private Transform _followPoint;

		void Update ()
		{
			if(_followPoint)
			{
				this.transform.position = _followPoint.position;
				this.transform.rotation = _followPoint.rotation;
			}
		}

		public Transform FollowTransform
		{
			get { return _followPoint; }
			set { _followPoint = value; }
		}
	}
}
#endregion