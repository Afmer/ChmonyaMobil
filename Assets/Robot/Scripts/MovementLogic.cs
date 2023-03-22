using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

//эта строчка гарантирует что наш скрипт не завалится 
//ести на плеере будет отсутствовать компонент Rigidbody
namespace OmniRobot
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementLogic : MonoBehaviour
    {
        public Vector3 SpeedVector { get; private set; }
        private Vector3 _movementVector;
        private Vector3 _rotationVector;
        public float AngleSpeed = 10f;
        public float FrictionCoef = 0.5f;
        public float MaxSpeed;
        public float Mass
        {
            get
            {
                return _rb.mass;
            }
        }
        [SerializeField] public float Strength;
        private Vector3 _strengthVector;
        public Vector3 StrengthDirection
        {
            get
            {
                return _strengthVector.normalized;
            }
            set
            {
                _strengthVector = value.normalized * Strength;
            }
        }
        private Vector3 _vectorAcceleration = new Vector3(0, 0, 0);
        public bool IsRotationCoroutineActive { get; private set; } = false;
        public Vector3 RotationVector
        {
            get { return _rotationVector; }
            set
            {
                if (!IsRotationCoroutineActive)
                    _rotationVector = Vector3.Normalize(value);
            }
        }
        public Vector3 MovementVector
        {
            get
            {
                return _movementVector;
            }
            set
            {
                _movementVector = Vector3.Normalize(value);
            }
        }
        private Rigidbody _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();

        }

        // обратите внимание что все действия с физикой 
        // необходимо обрабатывать в FixedUpdate, а не в Update
        void FixedUpdate()
        {
            MovementLogicFunc();
            if (!IsRotationCoroutineActive)
                RotationLogicFunc();
        }

        private void MovementLogicFunc()
        {
            _vectorAcceleration = _strengthVector / _rb.mass;
            if (_strengthVector.magnitude == 0)
            {
                var frictionStrength = FrictionCoef * _rb.mass * 9.8f;
                var frictionAcceleration = Vector3.Normalize(SpeedVector) * -1 * frictionStrength / _rb.mass;
                var tempSpeedVector = SpeedVector + frictionAcceleration * Time.deltaTime;
                if (Vector3.Normalize(tempSpeedVector) == Vector3.Normalize(SpeedVector))
                {
                    SpeedVector = tempSpeedVector;
                }
                else
                    SpeedVector = new Vector3(0, 0, 0);
            }
            else
            {
                SpeedVector += _vectorAcceleration * Time.deltaTime;
                float tempX;
                float tempY;
                float tempZ;
                if (Mathf.Abs(SpeedVector.x) > MaxSpeed)
                {
                    if (SpeedVector.x > 0)
                        tempX = MaxSpeed;
                    else
                        tempX = -MaxSpeed;
                }
                else
                {
                    tempX = SpeedVector.x;
                }
                if (Mathf.Abs(SpeedVector.y) > MaxSpeed)
                {
                    if (SpeedVector.y > 0)
                        tempY = MaxSpeed;
                    else
                        tempY = -MaxSpeed;
                }
                else
                {
                    tempY = SpeedVector.y;
                }
                if (Mathf.Abs(SpeedVector.z) > MaxSpeed)
                {
                    if (SpeedVector.z > 0)
                        tempZ = MaxSpeed;
                    else
                        tempZ = -MaxSpeed;
                }
                else
                {
                    tempZ = SpeedVector.z;
                }
                SpeedVector = new Vector3(tempX, tempY, tempZ);
            }
            transform.Translate(SpeedVector * Time.deltaTime);
            //_rb.AddRelativeForce(SpeedVector);
        }

        private IEnumerator RotateCoroutine(float degrees)
        {
            float currentRotation = 0;
            float direction = degrees / Mathf.Abs(degrees);
            float rotation;
            _rotationVector = new Vector3(0, direction, 0);
            Func<float, float, bool> boolFunc;
            if (degrees >= 0)
                boolFunc = (currentRotation, degrees) => { return currentRotation < degrees; };
            else
                boolFunc = (currentRotation, degrees) => { return currentRotation > degrees; };
            while (boolFunc(currentRotation, degrees))
            {
                yield return null;
                rotation = direction * AngleSpeed * Time.deltaTime;
                if (boolFunc(currentRotation + rotation, degrees))
                {
                    currentRotation += rotation;
                    transform.Rotate(new Vector3(0, rotation, 0));
                }
                else
                {
                    var finalRotation = degrees - currentRotation;
                    transform.Rotate(new Vector3(0, finalRotation, 0));
                    currentRotation = degrees;
                }
            }
            IsRotationCoroutineActive = false;
            _rotationVector = new Vector3();
            yield break;
        }

        public void Rotate(float degrees)
        {
            if (!IsRotationCoroutineActive)
            {
                IsRotationCoroutineActive = true;
                StartCoroutine(RotateCoroutine(degrees));
            }
            else
                throw new System.Exception("Rotation coroutine is active");
        }

        private void RotationLogicFunc()
        {
            transform.Rotate(RotationVector * AngleSpeed * Time.deltaTime);
        }
    }
}