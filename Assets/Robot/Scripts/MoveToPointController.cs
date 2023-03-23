using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kilosoft.Tools;
using OmniRobot;

public class MoveToPointController : MovePoint
{
    [SerializeField] private Transform _point;
    [EditorButton("Move")]
    public void Move()
    {
        MoveToPoint(_point.position);
        //_movementLogic.StrengthDirection = new Vector3(2.375f, 0, 1);
    }
}
