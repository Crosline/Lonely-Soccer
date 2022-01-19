using UnityEngine;

public class PurpleDummy : MonoBehaviour {

    public Vector3 initialPosition { get; private set; }
    void Start() {
        initialPosition = transform.position;
    }

}
