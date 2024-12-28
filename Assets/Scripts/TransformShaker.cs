using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class TransformShaker : MonoBehaviour {
    private readonly float _delay = .05f;
    private readonly float _distance = 6f;

    private readonly float _duration = .15f;
    private readonly Random rnd = new();


    public void BeginShake(Transform theTransform) {
        BeginShake(theTransform, _duration, _delay, _distance);
    }

    private void BeginShake(Transform theTransform, float duration, float delay, float distance) {
        StopAllCoroutines();
        StartCoroutine(Shake(theTransform, duration, delay, distance));
    }

    public async Task ABeginSpin(Transform theTransform, float duration, int rotations, int axis, bool isForward) {
        print("ABeginSpin");
        await ASpin(theTransform, duration, rotations, axis, isForward);
    }

    public async Task ABeginRandomSpin(Transform theTransform, float duration, int rotations) {
        var axis = rnd.Next(1, 4);
        var directionForward = rnd.Next(0, 2) == 0;
        await ABeginSpin(theTransform, duration, rotations, axis, directionForward);
    }


    private IEnumerator Shake(Transform theTransform, float duration, float delay, float distance) {
        print("ShakeTransform.Shake duration " + duration + " delay " + delay + " distance " +
              distance + "\n");
        var _startPos = theTransform.position;
        var elapsedTime = 0f;
        var numShakes = 0;
        while (elapsedTime < duration) {
            numShakes++;
            var deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;
            // print("ShakeTransform.Shake elapsedTime " + elapsedTime + " deltaTime " + deltaTime + "\n");
            var _randomPos = _startPos + UnityEngine.Random.insideUnitSphere * distance;
            theTransform.position = _randomPos;
            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            else
                yield return null;
        }

        theTransform.position = _startPos;
        print("ShakeTransform.Shake end " + numShakes + "\n");
    }


    public async Task ASpin(Transform theTransform, float duration, int rotations, int axis, bool isForward) {
        print("ShakeTransform.ASpin duration " + duration + " axis " + axis + " isForward " + isForward + "\n");
        var startRotation = theTransform.eulerAngles.x;
        var direction = isForward ? -360.0f : 360.0f; // minus goes fwd, + bwd
        var endRotation = startRotation + direction;
        var eulerx = theTransform.eulerAngles.x;
        var eulery = theTransform.eulerAngles.y;
        var eulerz = theTransform.eulerAngles.z;
        for (var i = 0; i < rotations; i++) {
            var t = 0.0f;
            while (t < duration) {
                t += Time.deltaTime;
                var theRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
                if (axis == 1)
                    theTransform.eulerAngles = new Vector3(eulerx, theRotation, eulerz);
                else if (axis == 2)
                    theTransform.eulerAngles = new Vector3(theRotation, eulery, eulerz);
                else if (axis == 3)
                    theTransform.eulerAngles = new Vector3(eulerx, eulery, theRotation);
                await Task.Yield();
            }
        }
    }
}
// public void BeginRandomSpin(Transform theTransform, float duration, int rotations) {
//     var axis = rnd.Next(1, 4);
//     var directionForward = rnd.Next(0, 2) == 0;
//     BeginSpin(theTransform, duration, rotations, axis, directionForward);
// }

// private void BeginSpin(Transform theTransform, float duration, int rotations, int axis, bool isForward) {
//     print("BeginSpin");
//     StopAllCoroutines();
//     StartCoroutine(Spin(theTransform, duration, rotations, axis, isForward));
// }

// private IEnumerator Spin(Transform theTransform, float duration, int rotations, int axis, bool isForward) {
//     print("ShakeTransform.Spin duration " + duration + " axis " + axis + " isForward " + isForward + "\n");
//     var startRotation = theTransform.eulerAngles.x;
//     var direction = isForward ? -360.0f : 360.0f; // minus goes fwd, + bwd
//     var endRotation = startRotation + direction;
//     var eulerx = theTransform.eulerAngles.x;
//     var eulery = theTransform.eulerAngles.y;
//     var eulerz = theTransform.eulerAngles.z;
//     for (var i = 0; i < rotations; i++) {
//         var t = 0.0f;
//         while (t < duration) {
//             t += Time.deltaTime;
//             var theRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
//             if (axis == 1)
//                 theTransform.eulerAngles = new Vector3(eulerx, theRotation, eulerz);
//             else if (axis == 2)
//                 theTransform.eulerAngles = new Vector3(theRotation, eulery, eulerz);
//             else if (axis == 3)
//                 theTransform.eulerAngles = new Vector3(eulerx, eulery, theRotation);
//             yield return null;
//         }
//     }
// }