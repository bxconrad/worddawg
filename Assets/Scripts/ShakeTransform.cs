using System.Collections;
using UnityEngine;

public class ShakeTransform : MonoBehaviour {
    private readonly float _delay = .05f;
    private readonly float _distance = 6f;
    private readonly float _shakeTime = .15f;

    public void Begin(Transform transformToShake) {
        Begin(transformToShake, _shakeTime, _delay, _distance);
    }

    public void Begin(Transform transformToShake, float shakeTime, float delay, float distance) {
        StopAllCoroutines();
        StartCoroutine(Shake(transformToShake, shakeTime, delay, distance));
    }


    private IEnumerator Shake(Transform transformToShake, float shakeTime, float delay, float distance) {
        print("ShakeTransform.Shake shakeTime " + shakeTime + " delay " + delay + " distance " +
              distance + "\n");
        var _startPos = transformToShake.position;
        var elapsedTime = 0f;
        var numShakes = 0;
        while (elapsedTime < shakeTime) {
            numShakes++;
            var deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;
            // print("ShakeTransform.Shake elapsedTime " + elapsedTime + " deltaTime " + deltaTime + "\n");
            var _randomPos = _startPos + Random.insideUnitSphere * distance;
            transformToShake.position = _randomPos;
            if (delay > 0f) {
                yield return new WaitForSeconds(delay);
            }
            else {
                yield return null;
            }
        }
        transformToShake.position = _startPos;
        print("ShakeTransform.Shake end " + numShakes + "\n");
    }
}