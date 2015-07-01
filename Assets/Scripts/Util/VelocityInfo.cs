using UnityEngine;
using System.Collections;
#region ERIC
/*
 * Information about an object's rigidbody
 * Used for pausing and unpausing
 */
namespace Assets.Scripts.Util
{
public class VelocityInfo : MonoBehaviour
{
        //is rigidbody kinematic?
        public bool _kinematic = false;

        //is rigidbody fixed angle?
        public bool _freezeRotation = false;

        //is rigidbody paused?
        private bool _paused = false;

        //reference to old velocity
        private Vector3 _vel = Vector3.zero;
        //reference to spinning velocity
        private Vector3 _angVel = Vector3.zero;

        //pause rigidbody
        public void PauseMotion()
        {
            if (!_kinematic && !_freezeRotation)
            {
                //save velocity
                _vel = this.GetComponent<Rigidbody>().velocity;
                //save angular velocity
                _angVel = this.GetComponent<Rigidbody>().angularVelocity;

                //set fixed angle
                this.GetComponent<Rigidbody>().freezeRotation = true;
                //set to kinematic to pause
                this.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (!_kinematic)
            {
                //save velocity
                _vel = this.GetComponent<Rigidbody>().velocity;
                //set to kinematic to pause
                this.GetComponent<Rigidbody>().isKinematic = true;
            }

            //pause
            _paused = true;
        }

        public void UnpauseMotion()
        {
            if (!_kinematic && !_freezeRotation)
            {
                //set to not kinematic to unpause
                this.GetComponent<Rigidbody>().isKinematic = false;

                //set to not fixed angle
                this.GetComponent<Rigidbody>().freezeRotation = false;
                //reapply angular velocity
                this.GetComponent<Rigidbody>().angularVelocity = _angVel;
                //reapply velocity
                this.GetComponent<Rigidbody>().velocity = _vel;

                //reset reference
                _angVel = Vector3.zero;
                //reset reference
                _vel = Vector3.zero;
            }
            else if (!_kinematic)
            {
                //set to not kinematic to unpause
                this.GetComponent<Rigidbody>().isKinematic = false;
                //reapply velocity
                this.GetComponent<Rigidbody>().velocity = _vel;
                //reset reference
                _vel = Vector3.zero;
            }

            //unpause
            _paused = false;
        }

        public bool Paused
        {
            get { return _paused; }
        }

        public Vector3 Vel
        {
            get { return _vel; }
        }

        public Vector3 AngVel
        {
            get { return _angVel; }
        }
    }
}
#endregion

