using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
namespace OmniRobot
{
    public abstract class MovePoint : MonoBehaviour
    {
        [SerializeField] public bool IsMoveWithRotation = true;
        protected Vector3 _targetPosition { get; private set; }
        public bool IsMovingToPoint { get; private set; } = false;
        [SerializeField] protected MovementLogic _movementLogic;
        protected Vector2 _targetVector
        {
            get
            {
                return new Vector2(_targetPosition.x - transform.position.x, _targetPosition.z - transform.position.z);
            }
        }


        private float GetAngleDegreesBetweenVectors(Vector2 direction, Vector2 targetVector)
        {
            var cos = (direction.x * targetVector.x + direction.y * targetVector.y) / (direction.magnitude * targetVector.magnitude);
            return RadiansToDegrees(Mathf.Acos(cos));
        }

        protected float RadiansToDegrees(float radians)
        {
            return radians * 57.2956f;
        }

        protected void MoveToPoint(Vector3 target)
        {
            if (!IsMovingToPoint)
            {
                IsMovingToPoint= true;
                _targetPosition = target;
                if (IsMoveWithRotation)
                    MoveWithRotation();
                else
                    MoveWithoutRotation();
            }
            else throw new Exception("Move is active");
        }

        private void MoveWithRotation()
        {
            var tempTargetVectorLocal = transform.InverseTransformVector(new Vector3(_targetVector.x, 0, _targetVector.y));
            var targetDirectionLocal = new Vector2(tempTargetVectorLocal.x, tempTargetVectorLocal.z);
            var forwardDirectionLocal = new Vector2(0, 1);
            var angle = GetAngleDegreesBetweenVectors(targetDirectionLocal, forwardDirectionLocal);
            if (targetDirectionLocal.x < 0)
                angle *= -1;
            _movementLogic.Rotate(angle);
            StartCoroutine(WaitingRotation(new Vector3(0, 0, 1)));
        }

        private void MoveWithoutRotation()
        {
            var targetVectorLocal = transform.InverseTransformVector(new Vector3(_targetVector.x, 0, _targetVector.y));
            StartCoroutine(ShortMoveCoroutine(targetVectorLocal));
        }

        private IEnumerator ShortMoveCoroutine(Vector3 direction)
        {
            float residualPath = _targetVector.magnitude;
            float brakeAcceleration = _movementLogic.FrictionCoef * 9.8f;
            Func<bool> isNeedToStop = () =>
            {
                float time = _movementLogic.SpeedVector.magnitude / (brakeAcceleration);
                bool result = (_movementLogic.SpeedVector.magnitude * time) - ((brakeAcceleration * Mathf.Pow(time, 2)) / 2) >= residualPath;
                return result;
            };
            while (!isNeedToStop())
            {
                yield return null;
                _movementLogic.StrengthDirection = direction.normalized;
                residualPath -= _movementLogic.SpeedVector.magnitude * Time.deltaTime;
            }
            _movementLogic.StrengthDirection = new Vector3(0, 0, 0);
            while (_movementLogic.SpeedVector.magnitude != 0)
                yield return null;
            IsMovingToPoint = false;
            yield break;
        }
        private IEnumerator WaitingRotation(Vector3 direction)
        {
            while (true)
            {
                if (!_movementLogic.IsRotationCoroutineActive)
                {
                    StartCoroutine(ShortMoveCoroutine(direction));
                    yield break;
                }
                yield return null;
            }
        }
    }
}