using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kilosoft.Tools;
namespace BezierScripts
{
    public class BezierLine : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private BezierSegment[] _segments;
        [SerializeField][HideInInspector] private GameObject _segmentsGameObject;
        [SerializeField] private Transform[] _points;
        public int SegmentsNums
        {
            get
            {
                return _segments.Length;
            }
        }

        [EditorButton("ApplyPoints")]
        public void ApplyPoints()
        {
            if (_points.Length != 0)
            {
                if (_segments != null)
                {
                    foreach (var segment in _segments)
                    {
                        segment.Clear();
                        DestroyImmediate(segment.gameObject);
                    }
                    DestroyImmediate(_segmentsGameObject);
                }
                _segments = new BezierSegment[_points.Length - 1];
                _segmentsGameObject = new GameObject("Segments");
                _segmentsGameObject.transform.parent = transform;

                for (int i = 0; i < _points.Length - 1; i++)
                {
                    var segment = new GameObject("Segment " + (i).ToString());
                    segment.transform.position = _points[i].transform.position;
                    segment.AddComponent<BezierSegment>();
                    var bezierSegment = segment.GetComponent<BezierSegment>();
                    bezierSegment.InitializeSegment(_points[i], _points[i + 1]);
                    segment.transform.parent = _segmentsGameObject.transform;
                    _segments[i] = bezierSegment;
                }
            }
        }
        public Vector3 GetPointToSegmentIndex(int segmentIndex, float capacity)
        {
            var result = _segments[segmentIndex].GetPoint(capacity);
            return result;
        }
        public Vector3 GetRotationToSegmentIndex(int segmentIndex, float capacity)
        {
            var result = _segments[segmentIndex].GetRotation(capacity);
            return result;
        }
    }
}