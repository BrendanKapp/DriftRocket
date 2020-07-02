using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueWrite : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Text[] reponses;
    private string toWrite;
    private int currentChar;

    private void Awake()
    {
        text = GetComponent<Text>();
    }
    public void SetWrite (DialogueEntry entry)
    {
        this.toWrite = entry.entry;
        //update this later to work with loaded writing
        for (int i = 0; i < 3; i++)
        {
            reponses[i].text = entry.responses[i];
        }
        StopCoroutine(Write());
        StartCoroutine(Write());
    }
    private IEnumerator Write()
    {
        for (int i = 0; i < toWrite.Length; i++)
        {
            currentChar++;
            string writeable = toWrite.Substring(0, currentChar);
            text.text = writeable + (i % 2 == 0 ? "|" : " ");
            yield return new WaitForSeconds(0.05f);
        }
    }
}
