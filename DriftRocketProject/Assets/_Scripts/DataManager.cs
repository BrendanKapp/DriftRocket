using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public delegate void SaveAction();
    public static event SaveAction SaveEvent;
    public delegate void LoadAction();
    public static event LoadAction LoadEvent;

    private void Start()
    {
        Load();
    }
    public static void SaveData (string name, float data)
    {
        PlayerPrefs.SetFloat(name, data);
    }
    public static float GetData (string name)
    {
        return PlayerPrefs.GetFloat(name);
    }
    private static void Save()
    {
        SaveEvent();
        PlayerPrefs.Save();
    }
    private static void Load()
    {
        LoadEvent();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}
