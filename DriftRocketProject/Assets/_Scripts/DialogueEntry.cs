using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEntry : MonoBehaviour
{
    public int id; //useless, just for showing ease of record keeping
    public string speaker; //who is speaking, a reference to the speaker db
    public bool IsProtagonist
    {
        get
        {
            return speaker == "Player";
        }
    }
    public string entry; //what is this person saying
    public string[] responses; //the responses that will lead to the children dialogue choices
    public DialogueEntry[] children; //the corresponding children to the responses
    private DialogueEntry parent; //the parent entry, for tranversing up the tree
}
