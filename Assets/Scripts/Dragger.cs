using UnityEngine;
using UnityEngine.EventSystems;

public class Dragger : MonoBehaviour, IDragHandler, IEndDragHandler {
    private float firstPos;
    private bool isFirstDrag = true;

    private UpdateBoard updateBoard;

    private void Awake() {
        updateBoard = GetComponentInParent<UpdateBoard>();
    }

    public void OnDrag(PointerEventData eventData) {
        //print("InputWord.OnDrag " + eventData.position.x + "\n");
        if (isFirstDrag) {
            isFirstDrag = false;
            firstPos = eventData.position.x;
            //print("Dragger.OnDrag setting " + eventData.position.x + isFirstDrag + "\n");
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var distance = eventData.position.x - firstPos;
        print("Dragger.OnEndDrag pos " + eventData.position.x + " distance " + distance + " firstpos " + firstPos +
              "\n");
        isFirstDrag = true;
        if (distance < -75) {
            updateBoard.CancelUpdateButton();
        }
    }
}