using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class MovePoint : MonoBehaviour
{
    private Vector3 _targetPosition
    {
        get
        {
            return TargetTransform.position;
        }
    }
    private Vector2 _basisX = new Vector2(1, 0);
    private bool temp = true;
    private Vector2 _forwardDirectionVector
    {
        get
        {
            return new Vector2(_forwardDirection.position.x - transform.position.x, _forwardDirection.position.z - transform.position.z).normalized;
        }
    }
    [SerializeField] private MovementLogic _movementLogic;
    [SerializeField] private Transform _forwardDirection;
    [SerializeField] public Transform TargetTransform;
    public Vector2 TargetVector
    {
        get
        {
            return new Vector2(_targetPosition.x - transform.position.x, _targetPosition.z - transform.position.z);
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (temp)
        {
            MoveToPoint();
            //_movementLogic.Rotate(-90);
            temp = false;
        }
    }

    private float GetAngleDegreesBetweenVectors(Vector2 direction, Vector2 targetVector)
    {
        var cos = (direction.x * targetVector.x + direction.y * targetVector.y) / (direction.magnitude * targetVector.magnitude);
        return RadiansToDegrees(Mathf.Acos(cos));
    }

    private float RadiansToDegrees(float radians)
    {
        return radians * 57.2956f;
    }

    public void MoveToPoint()
    {
        var angle = GetAngleDegreesBetweenVectors(_forwardDirectionVector, _basisX) - GetAngleDegreesBetweenVectors(TargetVector.normalized, _basisX);
        //var angle = RadiansToDegrees(Mathf.Atan((TargetVector.x - _forwardDirectionVector.x) / (TargetVector.y - _forwardDirectionVector.y)));
        _movementLogic.Rotate(angle);
        float acceleration = _movementLogic.Strength / _movementLogic.Mass;
        float brakeAcceleration = _movementLogic.FrictionCoef * 9.8f;
        float timeForAcceleration = _movementLogic.MaxSpeed / acceleration;
        float timeBrake = _movementLogic.MaxSpeed / brakeAcceleration;
        float accelerationPath = (acceleration * timeForAcceleration * timeForAcceleration) / 2;
        float brakePath = _movementLogic.MaxSpeed * timeBrake + ((brakeAcceleration * timeBrake * timeBrake) / 2);
        float path = TargetVector.magnitude;
        path -= (accelerationPath + brakePath);
        if (path >= 0)
        {
            float time = path / _movementLogic.MaxSpeed;
            float activeTime = timeForAcceleration + time;
            StartCoroutine(WaitingRotation(activeTime));
        }
        else
        {
            StartCoroutine(WaitingRotation());
        }
    }

    private IEnumerator MoveCoroutine(float time)
    {
        float currentTime = 0;
        while(currentTime < time)
        {
            yield return null;
            currentTime += Time.deltaTime;
            _movementLogic.StrengthDirection = new Vector3(0, 0, 1);
        }
        _movementLogic.StrengthDirection = new Vector3(0, 0, 0);
        yield break;
    }

    private IEnumerator ShortMoveCoroutine()
    {
        float residualPath = TargetVector.magnitude;
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
            _movementLogic.StrengthDirection = new Vector3(0, 0, 1);
            residualPath -= _movementLogic.SpeedVector.magnitude * Time.deltaTime;
        }
        _movementLogic.StrengthDirection = new Vector3(0, 0, 0);
        yield break;
    }

    private IEnumerator WaitingRotation(float activeTime)
    {
        while (true)
        {
            if (!_movementLogic.IsRotationCoroutineActive)
            {
                StartCoroutine(MoveCoroutine(activeTime));
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator WaitingRotation()
    {
        while (true)
        {
            if (!_movementLogic.IsRotationCoroutineActive)
            {
                StartCoroutine(ShortMoveCoroutine());
                yield break;
            }
            yield return null;
        }
    }
}
