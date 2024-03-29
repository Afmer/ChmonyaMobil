using BezierScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kilosoft.Tools;

namespace OmniRobot
{
    public class MoveBezierController : MovePoint
    {
        [SerializeField][Range(0.00001f, 1)] private float _step = 0.1f;
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
        [SerializeField] private BezierLine _bezierLine;
        private bool _isMoving = false;
        [EditorButton("Move")]
        public void Move()
        {
            if(Application.isPlaying)
            {
                if (!_isMoving)
                {
                    _isMoving = true;
                    StartCoroutine(MoveCoroutine());
                }
                else throw new System.Exception("bezier movement is active");
            }
        }

        private IEnumerator MoveCoroutine()
        {
            for(int i = 0; i < BezierLine.SegmentsNums; i++)
            {
                for(float j = 0; j < 1; j += _step)
                {
                    MoveToPoint(BezierLine.GetPointToSegmentIndex(i, j));
                    while(IsMovingToPoint)
                        yield return null;
                }
                MoveToPoint(BezierLine.GetPointToSegmentIndex(i, 1));
                while (IsMovingToPoint)
                    yield return null;
            }
            _isMoving = false;
        }
    }
}
