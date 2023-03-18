using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
namespace BezierScripts
{
    public class BezierSegment : MonoBehaviour
    {

        [SerializeField][HideInInspector] private Transform _P0;
        [SerializeField][HideInInspector] private Transform _P1;
        [SerializeField][HideInInspector] private Transform _P2;
        [SerializeField][HideInInspector] private Transform _P3;

        public Vector2 GetPoint(float t)
        {
            if (t > 1 && t < 0)
                throw new System.Exception("Incorrect T parameter");
            return Bezier.GetPoint(_P0.position, _P1.position, _P2.position, _P3.position, t);
        }

        public Vector3 GetRotation(float t)
        {
            if (t > 1 && t < 0)
                throw new System.Exception("Incorrect T parameter");
            return Bezier.GetFirstDerivative(_P0.position, _P1.position, _P2.position, _P3.position, t);
        }


        private void OnDrawGizmos()
        {

            int sigmentsNumber = 20;
            Vector3 preveousePoint = _P0.position;
            Gizmos.color = UnityEngine.Color.white;
            for (int i = 0; i < sigmentsNumber + 1; i++)
            {
                float paremeter = (float)i / sigmentsNumber;
                Vector3 point = Bezier.GetPoint(_P0.position, _P1.position, _P2.position, _P3.position, paremeter);
                Gizmos.DrawLine(preveousePoint, point);
                preveousePoint = point;
            }
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawLine(_P0.position, _P1.position);
            Gizmos.DrawLine(_P3.position, _P2.position);
        }

        public void InitializeSegment(Transform startPoint, Transform endPoint)
        {
            Texture2D iconTarget = EditorGUIUtility.IconContent("sv_icon_dot6_pix16_gizmo").image as Texture2D; //красная точка
            Texture2D iconController = EditorGUIUtility.IconContent("sv_icon_dot3_pix16_gizmo").image as Texture2D; //зеленая точка
            _P0 = startPoint;
            EditorGUIUtility.SetIconForObject(_P0.gameObject, iconTarget);
            _P3 = endPoint;
            EditorGUIUtility.SetIconForObject(_P3.gameObject, iconTarget);
            if (_P1 != null) DestroyImmediate(_P1.gameObject);
            _P1 = new GameObject("Controller 1").transform;
            EditorGUIUtility.SetIconForObject(_P1.gameObject, iconController);
            _P1.position = startPoint.transform.position;
            _P1.parent = startPoint.transform;
            _P1.transform.Translate(new Vector3(0, 0.1f, 0));
            if (_P2 != null) DestroyImmediate(_P2.gameObject);
            _P2 = new GameObject("Controller 2").transform;
            EditorGUIUtility.SetIconForObject(_P2.gameObject, iconController);
            _P2.position = endPoint.transform.position;
            _P2.parent = endPoint.transform;
            _P2.transform.Translate(new Vector3(0, 0.1f, 0));
        }

        public void Clear()
        {
            if (_P1 != null) DestroyImmediate(_P1.gameObject);
            if (_P2 != null) DestroyImmediate(_P2.gameObject);
        }
    }
}