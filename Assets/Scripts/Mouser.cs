using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouser : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private TextMeshProUGUI howToPlayText;
    private bool isOpen;

    public void OnPointerDown(PointerEventData eventData) {
        print("Mouser.OnPointerDown " + isOpen + "\n");
        isOpen = !isOpen;
        howToPlayText.gameObject.SetActive(isOpen);
    }
}