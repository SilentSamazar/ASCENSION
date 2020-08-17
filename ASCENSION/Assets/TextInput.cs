using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
{
    public InputField inputField;
    public GameObject UIController;
    public string LatestUnusedInput;
    
    void Awake()
    {
        UIController = GameObject.Find("UIController");
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

    void AcceptStringInput(string userInput)
    {
        userInput.ToLower();
        LatestUnusedInput = userInput;

        if (!string.IsNullOrWhiteSpace(userInput)) // Logs everything that isn't just the player hitting enter.
        {
            userInput = "> " + userInput;
            UIController.GetComponent<UIStuff>().LogStringWithReturn(userInput);
            InputComplete();
        }

        // Also perform the pass to other functions here, such that "enter" can continue etc.
        inputField.ActivateInputField();//This will lock you into the input field
    }

    void InputComplete()
    {
        UIController.GetComponent<UIStuff>().DisplayLoggedText();
        inputField.text = null;
    }

    string GetLatest() // UNDER CONSTRUCTION
    {
        
        string Input = LatestUnusedInput;
        LatestUnusedInput = null;
        return Input;
    }

}
