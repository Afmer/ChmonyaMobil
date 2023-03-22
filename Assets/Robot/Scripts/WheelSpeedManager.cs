using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OmniRobot
{
    public class WheelSpeedManager : MonoBehaviour
    {
        [SerializeField] private MovementLogic _movementLogic;
        [SerializeField] private GameObject _forwardWheel;
        [SerializeField] private GameObject _leftWheel;
        [SerializeField] private GameObject _rightWheel;
        private WheelSpeed _forwardSpeed;
        private WheelSpeed _leftSpeed;
        private WheelSpeed _rightSpeed;
        private float _radius;
        private float _radiusFromCenterMass;
        private float _vx;
        private float _w;
        private float _vy;
        void Start()
        {
            WheelRadius radius;
            if (_forwardWheel.TryGetComponent<WheelRadius>(out radius))
            {
                _radius = radius.Radius;
            }
            else
            {
                throw new Exception("radius is null");
            }
            _radiusFromCenterMass = (transform.position - _forwardWheel.transform.position).magnitude;
            if (!_forwardWheel.TryGetComponent<WheelSpeed>(out _forwardSpeed))
                throw new Exception("wheel speed exception");
            if (!_leftWheel.TryGetComponent<WheelSpeed>(out _leftSpeed))
                throw new Exception("wheel speed exception");
            if (!_rightWheel.TryGetComponent<WheelSpeed>(out _rightSpeed))
                throw new Exception("wheel speed exception");
        }
        void FixedUpdate()
        {
            _vx = (_movementLogic.SpeedVector).x;
            _vy = (_movementLogic.SpeedVector).z;
            _w = DegreesToRadians((_movementLogic.RotationVector * _movementLogic.AngleSpeed).y);
            _forwardSpeed.AngleSpeed = GetForwardAngleSpeed();
            _leftSpeed.AngleSpeed = GetLeftAngleSpeed();
            _rightSpeed.AngleSpeed = GetRightAngleSpeed();
        }

        private float GetForwardAngleSpeed()
        {
            return RadiansToDegrees(-(_vx + _w * _radiusFromCenterMass) / _radius);
        }

        private float GetRightAngleSpeed()
        {
            return RadiansToDegrees(-(-_vx * (float)Math.Cos(Math.PI / 3) - _vy * (float)Math.Sin(Math.PI / 3) + _w * _radiusFromCenterMass) / _radius);
        }
        private float GetLeftAngleSpeed()
        {
            return RadiansToDegrees(-(-_vx * (float)Math.Sin(Math.PI / 6) + _vy * (float)Math.Cos(Math.PI / 6) + _w * _radiusFromCenterMass) / _radius);
        }

        private float DegreesToRadians(float degrees)
        {
            return degrees / 57.2956f;
        }

        private float RadiansToDegrees(float radians)
        {
            return radians * 57.2956f;
        }
    }
}