using UnityEngine;
using UnityEngine.UI;

public class WordGrid : MonoBehaviour {
    [SerializeField] private GameObject displayButtonPrefab;
    [SerializeField] private UpdateBoard updateBoard;

    public void Initialize() {
        print("WordGrid.Initialize beforeDestroy \n");
        foreach (Transform child in transform) {
            //print("WordGrid.DestroyDisplayButtons destroying " + child.gameObject.name + "\n");
            Destroy(child.gameObject);
        }
    }

    // If we created a new word, instantiate
    // if we updated an existing word, find it and change it to have the new word
    public void UpdateDisplayButton(string originalWord, string newWord) {
        print("WordGrid.UpdateDisplayButton oWord " + originalWord + " nWord " + newWord + "\n");

        // If there was no originalWord that means we are creating a new word
        if (originalWord.Length == 0) {
            InstantiateDisplayButton(newWord);
        }
        // If there was an originalWord that means we are updating an existing word
        else {
            UpdateExistingWord(originalWord, newWord);
        }
    }

    private void InstantiateDisplayButton(string newWord) {
        var newDisplayButton = Instantiate(displayButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newDisplayButton.transform.SetParent(transform, false);
        var displayButton = newDisplayButton.GetComponent<DisplayButton>();
        displayButton.SetWord(newWord);
        newDisplayButton.transform.SetAsFirstSibling();
        newDisplayButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnButtonClick(newWord));
        print("WordGrid.InstantiateDisplayButton " + newWord + "\n");
    }

    // An existing word was updated.
    // Find it in our list of words. 
    // Change the text and the onClick to reflect the new word
    private void UpdateExistingWord(string originalWord, string newWord) {
        print("WordGrid.UpdateExistingWord oWord " + originalWord + " nWord " + newWord + "\n");
        var displayButtons = GetComponentsInChildren<DisplayButton>(); //go up to parent and then from hier?

        for (var i = 0; i < displayButtons.Length; i++) {
            var displayButton = displayButtons[i];
            // Find the displayButton that was originally clicked to create a new word
            if (originalWord.Equals(displayButton.GetWord())) {
                // Update to have the new word 
                displayButton.SetWord(newWord);
                displayButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                displayButton.GetComponentInChildren<Button>().onClick
                    .AddListener(() => OnButtonClick(newWord));
                // Display the modified word at the top of the list of words
                displayButton.transform.SetAsFirstSibling();
                print("WordGrid. UpdateExistingWord " + newWord + "\n");
                return;
            }
            print("WordGrid.UpdateExistingWord " + newWord + "\n");
        }
    }

    public void OnButtonClick(string buttonName) {
        print("WordGrid.OnButtonClick  name " + buttonName + "\n");
        updateBoard.LoadSelectedWord(buttonName);
    }
}