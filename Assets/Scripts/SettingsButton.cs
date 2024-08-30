using EasyUI.Toast;
using UnityEngine;

public class SettingsButton : MonoBehaviour {
    [SerializeField] private GameObject prefsContainer;
    [SerializeField] private GameObject updateContainer;

    //bcdo move SettingsButton to FooterCanvas and disable updateCanvas
    public void OnButtonClickSettings() {
        Toast.Dismiss();
        var isActive = prefsContainer.activeSelf;
        prefsContainer.SetActive(!isActive);
        updateContainer.SetActive(isActive);
    }
}