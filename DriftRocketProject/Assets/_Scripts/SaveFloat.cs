using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFloat
{
    public float value;

    private string key;

    public SaveFloat (string key)
    {
        this.key = key;
        LoadValue();
        DataManager.SaveEvent += SaveValue;
        DataManager.LoadEvent += LoadValue;
    }
    private void LoadValue()
    {
        value = PlayerPrefs.GetFloat(key);
    }
    private void SaveValue ()
    {
        PlayerPrefs.SetFloat(key, value);
    }
    ~SaveFloat ()
    {
        DataManager.SaveEvent -= SaveValue;
        DataManager.LoadEvent -= LoadValue;
    }
}
