using BezierScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmniRobot
{
    public class MoveBezierController : MovePoint
    {
        public BezierLine BezierLine
        {
            get
            {
                return _bezierLine;
            }
            set
            {
                if (!_isMoving)
                {
                    _bezierLine = value;
                }
                else throw new System.Exception("Robot is moving now");
            }
        }
        private BezierLine _bezierLine;
        private bool _isMoving = false;
    }
}
