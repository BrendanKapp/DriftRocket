using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    SerializedProperty protagonistSpeaker;
    SerializedProperty otherSpeaker;
    SerializedProperty startingEntry;

    private int id = 0;

    void OnEnable()
    {
        // Setup the SerializedProperties
        protagonistSpeaker = serializedObject.FindProperty("protagonistUI");
        otherSpeaker = serializedObject.FindProperty("otherSpeakerUI");
        startingEntry = serializedObject.FindProperty("startingEntry");
        DialogueManager dialogueManager = (DialogueManager)target;
        id = dialogueManager.GetComponentsInChildren<DialogueEntry>().Length;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DialogueManager dialogueManager = (DialogueManager)target;
        EditorGUILayout.PropertyField(protagonistSpeaker, new GUIContent("Protagonist UI"));
        EditorGUILayout.PropertyField(otherSpeaker, new GUIContent("Other Speaker UI"));
        EditorGUILayout.PropertyField(startingEntry, new GUIContent("Starting Dialogue Entry"));
        GUI.color = Color.green;
        if (GUILayout.Button("Add Dialogue", GUILayout.Height(50)))
        {
            DialogueEntry dialogueEntry = dialogueManager.gameObject.AddComponent<DialogueEntry>();
            dialogueEntry.id = id++;
        }
        GUI.color = Color.white;
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
