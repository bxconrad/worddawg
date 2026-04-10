using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGrid : MonoBehaviour {
    [SerializeField] private GameObject displayButtonPrefab;
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private Image viewPortImage;
    public DisplayButton selectedButton;

    public void Activate(bool isActive) {
        if (isActive) {
            viewPortImage.color = Color.magenta;
        }
        else {
            viewPortImage.color = Color.black;
        }
    }

    public bool DeselectMismatchButton(Word word) {
        if (selectedButton != null && !selectedButton.GetWord().Equals(word)) {
            selectedButton.DeSelectButton();
            print("~~WordGrid.DeselectMismatchButton deselecting  {" + selectedButton.GetWord().contents + "\n");

            return true;
        }

        return false;
    }

    public void UpdateGridLayoutConstraint(int count) {
        var gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        print("WordGrid.UpdateGridLayoutConstraint gridLayout {" + gridLayout + "\n");
        gridLayout.constraintCount = count;
    }

    public void Initialize() {
        print("WordGrid.Initialize beforeDestroy \n");
        foreach (Transform child in transform) {
            // print("WordGrid.Initialize destroying " + child.gameObject.name + "\n");
            Destroy(child.gameObject);
        }
    }


    // If we created a new word, instantiate
    // if we updated an existing word, find it and change it to have the new word
    //bcdo remove newWord from method call
    public void UpdateDisplayButton(Word wordObject) {
        print("WordGrid.UpdateDisplayButton oWord {" + wordObject + "}\n");

        // If there was no originalWord that means we are creating a new word
        if (wordObject.IsNewWord()) {
            InstantiateDisplayButton(wordObject);
        }
        // If there was an originalWord that means we are updating an existing word
        else {
            UpdateExistingWord(wordObject);
        }
    }

    public void InstantiateDisplayButton(Word word) {
        var newDisplayButton = Instantiate(displayButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newDisplayButton.transform.SetParent(transform, false);
        var displayButton = newDisplayButton.GetComponent<DisplayButton>();
        displayButton.SetWord(word);
        newDisplayButton.transform.SetAsFirstSibling();
        newDisplayButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnButtonClick(word));
        print("WordGrid.InstantiateDisplayButton " + word + "\n");
    }

    // An existing word was updated.
    // Find it in our list of words. 
    // Change the text and the onClick to reflect the new word
    private void UpdateExistingWord(Word wordObject) {
        print("WordGrid.UpdateExistingWord oWord " + wordObject + "\n");
        DeselectButton();
        var displayButton = FindMatchingButton(wordObject);
        if (displayButton != null) {
            displayButton.SetWord(wordObject);
            displayButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            displayButton.GetComponentInChildren<Button>().onClick
                .AddListener(() => OnButtonClick(wordObject));
            // Display the modified word at the top of the list of words
            displayButton.transform.SetAsFirstSibling();
            print("WordGrid. UpdateExistingWord \n");
        }
        else {
            print("WordGrid.UpdateExistingWord null\n");
        }
    }

    public DisplayButton FindMatchingButton(Word wordObject) {
        print("WordGrid.FindMatchingButton " + wordObject + "\n");
        var displayButtons = GetComponentsInChildren<DisplayButton>(); //go up to parent and then from hier?

        for (var i = 0; i < displayButtons.Length; i++) {
            var displayButton = displayButtons[i];
            // Find the displayButton that was originally clicked to create a new word
            if (wordObject.Equals(displayButton.GetWord())) {
                // Update to have the new word 
                print("WordGrid.FindMatchingButton found " + displayButton + " i: " + i + "\n");
                return displayButton;
            }
        }

        print("WordGrid.FindMatchingButton ***NOT*** found " + wordObject + "\n");
        return null;
    }

    public void SetInteractable(bool isInteractable) {
        //print("WordGrid.SetInteractable " + isInteractable + "\n");
        var displayButtons = GetComponentsInChildren<DisplayButton>();
        foreach (var displayButton in displayButtons) {
            displayButton.SetInteractable(isInteractable);
        }
    }


    public List<string> FindWordList() {
        print("WordGrid.FindWordListt\n");
        var words = new List<string>();
        var displayButtons = GetComponentsInChildren<DisplayButton>(); //go up to parent and then from hier?

        foreach (var displayButton in displayButtons) {
            words.Add(displayButton.GetWordText());
        }

        return words;
    }

    public List<Word> FindWordObjects() {
        print("WordGrid.FindWordObjects\n");
        var words = new List<Word>();
        var displayButtons = GetComponentsInChildren<DisplayButton>(); //go up to parent and then from hier?

        foreach (var displayButton in displayButtons) {
            if (displayButton.enabled) {
                // ? buttons are there first time around even though they were destroyed
                words.Add(displayButton.GetWord());
            }
        }

        return words;
    }

    public DisplayButton FindDisplayButtonForWordObject(Word word) {
        print("WordGrid.FindDisplayButtonForWordObject\n");
        var displayButtons = GetComponentsInChildren<DisplayButton>();
        foreach (var displayButton in displayButtons) {
            if (displayButton.GetWord().Equals(word)) {
                return displayButton;
            }
        }

        return null;
    }

    public void DeselectButton() {
        print("WordGrid.DeselectButton  selectedButton {" + selectedButton + "}\n");
        if (selectedButton != null) {
            selectedButton.DeSelectButton();
            selectedButton = null;
        }
    }

    private void SelectButton(Word wordObject) {
        print("WordGrid.OnButtonClick  name " + wordObject + "\n");
        var displayButton = FindMatchingButton(wordObject);
        if (displayButton != null) {
            DeselectButton();
            selectedButton = displayButton;
            displayButton.SelectButton();
        }
    }

    private void OnButtonClick(Word wordObject) {
        print("WordGrid.OnButtonClick  name " + wordObject + "\n");
        SelectButton(wordObject);
        updateBoard.LoadSelectedWord(wordObject);
    }
}