using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kilosoft.Tools;
using OmniRobot;

public class RotationController : MonoBehaviour
{
    [SerializeField] private MovementLogic _movementLogic;
    [SerializeField] private float _angle;
    [EditorButton("Rotate")]
    public void Rotate()
    {
        _movementLogic.Rotate(_angle);
    }
}
