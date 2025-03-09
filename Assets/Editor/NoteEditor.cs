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

        EditorGUILayout.LabelField("Not Alaný", EditorStyles.boldLabel);

        // TextArea'nýn satýr doldukça alta geçmesi için GUI geniþliðini kullanýyoruz
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;  // Kelime kaydýrmayý açýyoruz

        noteProperty.stringValue = EditorGUILayout.TextArea(noteProperty.stringValue, textAreaStyle, GUILayout.Height(200));

        serializedObject.ApplyModifiedProperties();
    }
}
