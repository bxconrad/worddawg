using EasyUI.Toast;
using UnityEngine;
using UnityEngine.EventSystems;

public class LogoImage : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) {
            Toast.Dismiss();
        }
    }
}