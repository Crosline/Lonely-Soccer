using UnityEngine;

public class InputHandler : MonoBehaviour {

    [SerializeField]
    private float _sensitivity;

    private bool _isActive = false;

    private bool _isDragging = false;

    private Vector3 _touchPosition = Vector3.zero;
    private Vector3 _deltaTouchPosition = Vector3.zero;


    private Vector3 _startPos;

    private Transform _dragging;
    private Vector3 _dragStart;

    private void Init() {
        _deltaTouchPosition = Vector3.zero;
        _touchPosition = Vector3.zero;
        _dragging = null;
        _isDragging = false;
    }

    void Awake() {
        _startPos = transform.position;
        Init();
    }

    void Update() {
        if (!_isActive) return;


#if UNITY_EDITOR || UNITY_WIN_STANDALONE || UNITY_WEBGL || UNITY_STANDALONE_WIN

        if (Input.GetMouseButtonDown(0))
            if (!CheckDummy(Input.mousePosition)) {
                GameManager.Instance.ChangeState(GameManager.GameState.WaitingRun);
                _isDragging = true;
            }



        if (Input.GetMouseButton(0)) {
            var position = Input.mousePosition;

            if (_touchPosition == Vector3.zero)
                _touchPosition = position;

            _deltaTouchPosition = _touchPosition - position;

            if (_isDragging)
                RotateOnMovement(_deltaTouchPosition.x);

            if (_dragging != null) {

                _dragging.position -= Vector3.Project(new Vector3(_deltaTouchPosition.x, 0f, _deltaTouchPosition.y) * Time.deltaTime * 2f, _dragging.right);

                _dragging.position = new Vector3(Mathf.Clamp(_dragging.position.x, _dragStart.x - 2f, _dragStart.x + 2f), _dragging.position.y, Mathf.Clamp(_dragging.position.z, _dragStart.z - 2f, _dragStart.z + 2f));

            }

            _touchPosition = position;

        }
        if (Input.GetMouseButtonUp(0)) {
            if (_isDragging) {
                _isDragging = false;
                SendBall();
            }
            if (_dragging != null) {
                _dragging = null;
                Init();
            }

        }

#else
        if (Input.touchCount > 0) { // We are holding down currently
            Touch touch = Input.GetTouch(0);
            var position = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                if (!CheckDummy(Input.mousePosition)) {
                    GameManager.Instance.ChangeState(GameManager.GameState.WaitingRun);
                    _isDragging = true;
                }
            }

            Vector3 position = Input.mousePosition;

            if (_touchPosition == Vector3.zero)
                _touchPosition = position;

            _deltaTouchPosition = _touchPosition - position;

            if (_isDragging)
                RotateOnMovement(_deltaTouchPosition.x);

            if (_dragging != null) {

                _dragging.position -= Vector3.Project(new Vector3(_deltaTouchPosition.x, 0f, _deltaTouchPosition.y) * Time.deltaTime * 2f, _dragging.right);

                _dragging.position = new Vector3(Mathf.Clamp(_dragging.position.x, _dragStart.x - 2f, _dragStart.x + 2f), _dragging.position.y, Mathf.Clamp(_dragging.position.z, _dragStart.z - 2f, _dragStart.z + 2f));

            }

            _touchPosition = position;


        } else { // Releasing the touch
            if (_isDragging) {
                _isDragging = false;
                SendBall();
            }
            if (_dragging != null) {
                _dragging = null;
                Init();
            }
        }
#endif

    }

    public void ResetTransform() {
        transform.position = _startPos;
        transform.rotation = Quaternion.identity;
    }

    public void SetActive(bool isActive) {
        Init();
        _isActive = isActive;
    }

    private bool CheckDummy(Vector3 inputPosition) {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000)) {
            if (hit.collider.CompareTag("RedDummy")) {

                hit.collider.transform.Rotate(0f, 45f * 0.5f, 0f);


                return true;
            } else if (hit.collider.CompareTag("PurpleDummy")) {

                if (hit.collider.TryGetComponent<PurpleDummy>(out PurpleDummy purpleDummy)) {
                    _dragging = hit.collider.transform;

                    _dragStart = purpleDummy.initialPosition;

                    return true;
                }
            }
        }


        return false;
    }

    private void RotateOnMovement(float deltaMovement) {
        transform.RotateAround(transform.position - transform.forward * 0.5f, transform.up * -1f, deltaMovement * _sensitivity * Time.deltaTime);
    }

    private void SendBall() {

        GameManager.Instance.ChangeState(GameManager.GameState.Running);
    }
}
