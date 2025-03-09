using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Character))]
public class SO_CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Call the default inspector first
        base.OnInspectorGUI();

        // Get a reference to the current SpriteData instance
        SO_Character character = (SO_Character)target;

        // If there is a sprite assigned, show it in the inspector
        if (character.SplashArt != null)
        {
            GUILayout.Label("Sprite Preview:");
            // Show the sprite using GUILayout
            GUILayout.Box(character.SplashArt.texture, GUILayout.Width(100), GUILayout.Height(100));
        }
    }
}