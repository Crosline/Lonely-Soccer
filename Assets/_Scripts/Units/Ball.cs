using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private float _lerpSpeed;
    // Start is called before the first frame update
    public void Init(List<Vector3> hp) {
        StartCoroutine(PushBall(hp));
    }

    IEnumerator PushBall(List<Vector3> hitPoints) {

        for (int i = 1; i < hitPoints.Count; i++) {
            float distance = Vector3.Distance(hitPoints[i], transform.position);
            Vector3 startPos = transform.position;
            Vector3 newPos = hitPoints[i];
            if (i != hitPoints.Count - 1)
                newPos = new Vector3(hitPoints[i].x - 0.5f, hitPoints[i].y + Random.Range(-1f, 1f), hitPoints[i].z - 0.5f);
            for (float j = 0; j < distance; j += distance * _lerpSpeed * Time.deltaTime) {
                transform.position = Vector3.Lerp(startPos, newPos, j / distance);
                yield return null;

            }
            yield return null;
        }

        GameManager.Instance.ChangeState(GameManager.GameState.Win);
    }
}
