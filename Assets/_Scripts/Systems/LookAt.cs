using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    [SerializeField]
    private Transform _ball;

    private Vector3 _startPos;
    private Quaternion _startRotation;

    private bool _isFollowing = false;

    void Awake() {
        _startPos = transform.position;
        _startRotation = transform.rotation;
    }

    public void StartFollow() {
        _startPos = transform.position;
        _startRotation = transform.rotation;
        _isFollowing = true;

        if (_ball == null)
            _ball = GameObject.FindObjectOfType<Ball>().transform;

        _ball.rotation = Quaternion.identity;


        transform.rotation = Quaternion.Euler(0, 0, 0);

    }
    public void StopFollow() {
        _isFollowing = false;

        transform.position = _startPos;
        transform.rotation = _startRotation;

    }

    void Update() {
        if (!_isFollowing) return;

        transform.position = _ball.position + _ball.forward * -2f + Vector3.up *2f;




    }

}
