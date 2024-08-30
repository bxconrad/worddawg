using EasyUI.Toast;
using UnityEngine;
using UnityEngine.EventSystems;

public class LogoImage : MonoBehaviour, IPointerClickHandler {
    private ShakeTransform shakeTransform;

    public void OnPointerClick(PointerEventData eventData) {
        shakeTransform = transform.parent.parent.GetComponentInChildren<ShakeTransform>();
        if (eventData.clickCount == 2) {
            Toast.Dismiss();
            //     shakeTransform.Begin(transform);
        }
    }
}