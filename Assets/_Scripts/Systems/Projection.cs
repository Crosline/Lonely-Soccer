using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour {

    [SerializeField]
    private float _maxDistance;    

    [SerializeField]
    private LineRenderer _lineRenderer;

    public List<Vector3> hitPoints { get; private set; } = new List<Vector3>();

    public bool isGoalFound = false;

    void Start() {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.enabled = false;
    }
    public void GetTrajectoryLine() {
        hitPoints = GetHitPoints(transform);

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = hitPoints.Count;

        for (var i = 0; i < hitPoints.Count; i++) {
            _lineRenderer.SetPosition(i, hitPoints[i]);
        }
    }

    public void RemoveLine() {
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
    }

    private List<Vector3> GetHitPoints(Transform startingTransform) {

        Vector3 position = startingTransform.position;
        Vector3 direction = startingTransform.forward;

        List<Vector3> hitList = new List<Vector3>();

        hitList.Add(position);

        while (true) {
            Ray ray = new Ray(position, direction);

            if (Physics.Raycast(position, direction, out var hit,  _maxDistance)) {

                position = hit.point;
                hitList.Add(position);

                if (hit.collider.CompareTag("Goal")) {
                    isGoalFound = true;
                    break;
                } else {
                    isGoalFound = false;
                }

                direction = -2f * Vector3.Dot(hit.normal, direction) * hit.normal + direction;

            } else {

                hitList.Add(position + (direction * _maxDistance));
                break;
            }

        }

        return hitList;
    }


}
