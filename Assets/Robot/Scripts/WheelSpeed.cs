using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OmniRobot
{
    public class WheelSpeed : MonoBehaviour
    {
        public float AngleSpeed;

        private void FixedUpdate()
        {
            transform.Rotate(new Vector3(0, 0, AngleSpeed) * Time.deltaTime, Space.Self);
        }
    }
}
