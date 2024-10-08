using System.Collections;
using UnityEngine;

public class InputWordTile : BaseTile {
    public float startX;
    public float shakeWidth = 1f;
    public bool goingRight = true;
    public float speed = 20f;
    private NewInputWord newInputWord;

    protected new void Awake() {
        AwakeMe();
        newInputWord = GetComponentInParent<NewInputWord>();
        print("InputWordTile.Awake {" + newInputWord + "} \n");
    }

    public override void OnButtonClick(string buttonString) {
        print("InputWordTile.OnButtonClick \n");
        newInputWord.RemoveLetter(GetOriginTile());
        //StartCoroutine(CoroutineShake());
    }

    private IEnumerator CoroutineShake() {
        print("InputWordTile.CoroutineShake \n");
        if (goingRight) {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            if (transform.position.x - startX >= shakeWidth) {
                goingRight = false;
            }
        }
        else {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            if (transform.position.x <= startX) {
                goingRight = true;
            }
        }
        yield return null;
    }
}