using UnityEngine;

public class SettingsButton : MonoBehaviour {
    [SerializeField] private GameObject gameManagerContainer;

    public void Start() {
        gameManagerContainer.SetActive(true);
    }
}