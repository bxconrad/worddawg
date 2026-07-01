using System.Collections;
using UnityEngine;

public class InputWordTile : BaseTile {
    public float startX;
    public float shakeWidth = 1f;
    public bool goingRight = true;
    public float speed = 20f;
    private InputWord inputWord;

    protected override void Awake() {
        base.Awake();
        inputWord = GetComponentInParent<InputWord>();
        // print($"~InputWordTile.Awake {inputWord} \n");
    }

    public override void OnButtonClick(string buttonString) {
        print("~InputWordTile.OnButtonClick \n");
        inputWord.RemoveLetter(GetOriginTile());
        //StartCoroutine(CoroutineShake());
    }

    private IEnumerator CoroutineShake() {
        print("~InputWordTile.CoroutineShake \n");
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