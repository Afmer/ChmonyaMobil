using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OmniRobot
{
    public class WheelRadius : MonoBehaviour
    {
        [SerializeField] private Transform _endPoint;
        public float Radius { get; private set; }

        private void Awake()
        {
            Radius = (_endPoint.position - transform.position).magnitude;
        }
    }
}
