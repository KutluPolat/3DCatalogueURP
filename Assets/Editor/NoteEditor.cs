using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Note))]
public class NoteEditor : Editor
{
    SerializedProperty noteProperty;

    private void OnEnable()
    {
        noteProperty = serializedObject.FindProperty("note");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Not Alan�", EditorStyles.boldLabel);

        // TextArea'n�n sat�r dolduk�a alta ge�mesi i�in GUI geni�li�ini kullan�yoruz
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;  // Kelime kayd�rmay� a��yoruz

        noteProperty.stringValue = EditorGUILayout.TextArea(noteProperty.stringValue, textAreaStyle, GUILayout.Height(200));

        serializedObject.ApplyModifiedProperties();
    }
}
