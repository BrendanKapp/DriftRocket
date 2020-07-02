using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private DialogueWrite protagonistUI;
    [SerializeField]
    private DialogueWrite otherSpeakerUI;
    [SerializeField]
    private DialogueEntry startingEntry;
    private DialogueEntry currentEntry;

    public void ClickResponse(int responseID)
    {
        if (responseID >= currentEntry.children.Length)
        {
            Debug.LogError("Error: response " + responseID + " does not exist!");
            return;
        }
        currentEntry = currentEntry.children[responseID];
        UpdateDialogueUI();
    }
    private void UpdateDialogueUI()
    {
        if (currentEntry.IsProtagonist)
        {
            protagonistUI.SetWrite(currentEntry);
        } else
        {
            otherSpeakerUI.SetWrite(currentEntry);
        }
    }
}
