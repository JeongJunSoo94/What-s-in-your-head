using System;
using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class FlexPlatformObstacle : GroundSurface
    {
        [SerializeField] private float _areaOfInterest = 2;
        [SerializeField] private float _force = 2;
        [SerializeField] private float _displacementMultpilier = 1;
        [SerializeField] private float _animationSpeed = 5;
        [SerializeField] private MeshFilter _targetMeshFilter;

        private Vector3[] _originalVertices;
        private Vector3[] _displacedVertices;
        private Vector3[] _targetDisplacement;
        private Mesh _deformingMesh;

        private void Awake()
        {
            if(_targetMeshFilter == null)
            {
                _targetMeshFilter = GetComponent<MeshFilter>();
            }

            _deformingMesh = _targetMeshFilter.mesh;
            _originalVertices = _deformingMesh.vertices;
            _targetDisplacement = new Vector3[_originalVertices.Length];
            _displacedVertices = new Vector3[_originalVertices.Length];
            Array.Copy(_originalVertices, _displacedVertices, _originalVertices.Length);
        }

        public override void UpdateDisplacement(Vector3 point, Vector3 up, ref SurfaceDisplacement otherDisplacement)
        {
            ResetTargetDisplacement();

            for (var i = 0; i < _originalVertices.Length; i++)
            {
                var localHitPoint = transform.InverseTransformPoint(point);
                var distance = Vector3.Distance(localHitPoint, _originalVertices[i]);
                if (distance <= _areaOfInterest)
                {
                    var stepCoef = distance / _areaOfInterest;
                    var displacementForce = Mathf.SmoothStep(_force, 0, stepCoef);
                    if (_targetDisplacement[i].magnitude < displacementForce)
                    {
                        _targetDisplacement[i] = (_originalVertices[i] - localHitPoint).normalized * displacementForce * _displacementMultpilier;
                    }
                }
            }
        }

        /*
        public override void UpdateDisplacement(ref SurfaceHitInfo obstacleHit, ref SurfaceDisplacement thisDisplacement, ref SurfaceDisplacement otherDisplacement)
        {
            ResetTargetDisplacement();

            for (var i = 0; i < _originalVertices.Length; i++)
            {
                var localHitPoint = transform.InverseTransformPoint(obstacleHit.point);
                var distance = Vector3.Distance(localHitPoint, _originalVertices[i]);
                if (distance <= _areaOfInterest)
                {
                    var stepCoef = distance / _areaOfInterest;
                    var displacementForce = Mathf.SmoothStep(_force, 0, stepCoef);
                    if (_targetDisplacement[i].magnitude < displacementForce)
                    {
                        _targetDisplacement[i] = (_originalVertices[i] - localHitPoint).normalized * displacementForce * _displacementMultpilier;
                    }
                }
            }
        }
        */

        private void Update()
        {
            ApplyDisplacement();
        }

        private void ApplyDisplacement()
        {
            for (var i = 0; i < _targetDisplacement.Length; i++)
            {
                var targetPoint = _targetDisplacement[i] + _originalVertices[i];
                _displacedVertices[i] = Vector3.Slerp(_displacedVertices[i], targetPoint, Time.deltaTime * _animationSpeed);
            }

            _deformingMesh.vertices = _displacedVertices;
            _deformingMesh.RecalculateNormals();

            ResetTargetDisplacement();
        }

        private void ResetTargetDisplacement()
        {
            for (int i = 0; i < _targetDisplacement.Length; i++)
            {
                _targetDisplacement[i] = Vector3.zero;
            }
        }
    }
}
