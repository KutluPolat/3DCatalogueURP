using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KutluHelper : EditorWindow
{
    private int numColumns = 5;
    private int numRows = 100;
    private float horizontalSpacing = 0f;
    private float verticalSpacing = 0f;
    private string componentName = "";
    private string LabelSeperator => "__________________________________________________________________________________";

    [MenuItem("KUTLU/Show Kutlu Helper")]
    public static void ShowWindow()
    {
        // Open the window, or focus it if it's already open
        var window = GetWindow<KutluHelper>();
        window.titleContent = new GUIContent("KUTLU HELPER");
        window.Show();
    }

    private void OnGUI()
    {
        // Create an Example Button
        if (GUILayout.Button("VALIDATE CHARs"))
        {
            CheckResources_Characters();
            Debug.Log("Check character resource is complete!");
        }
        GUILayout.Label(LabelSeperator, EditorStyles.boldLabel);
        //GUI.enabled = Selection.gameObjects.Length >= 2;
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("XZ EVEN", GUILayout.Height(40)))
        {
            DistributeSelectedObjectsEvenly();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("X AVG"))
        {
            AlignOnAxisUsingBounds(Axis.X, Alignment.Average);
        }

        if (GUILayout.Button("X MIN"))
        {
            AlignOnAxisUsingBounds(Axis.X, Alignment.Min);
        }

        if (GUILayout.Button("X MAX"))
        {
            AlignOnAxisUsingBounds(Axis.X, Alignment.Max);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Z AVG"))
        {
            AlignOnAxisUsingBounds(Axis.Z, Alignment.Average);
        }

        if (GUILayout.Button("Z MIN"))
        {
            AlignOnAxisUsingBounds(Axis.Z, Alignment.Min);
        }

        if (GUILayout.Button("Z MAX"))
        {
            AlignOnAxisUsingBounds(Axis.Z, Alignment.Max);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Label(LabelSeperator, EditorStyles.boldLabel);

        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        numColumns = EditorGUILayout.IntField("Number of Columns", numColumns);
        numRows = EditorGUILayout.IntField("Number of Rows", numRows);
        horizontalSpacing = EditorGUILayout.FloatField("Horizontal Spacing", horizontalSpacing);
        verticalSpacing = EditorGUILayout.FloatField("Vertical Spacing", verticalSpacing);

        //GUI.enabled = Selection.gameObjects.Length >= 2;
        if (GUILayout.Button("Arrange Selected Objects"))
        {
            ArrangeSelectedObjects();
        }

        GUILayout.Label(LabelSeperator, EditorStyles.boldLabel);
        //GUI.enabled = Selection.gameObjects.Length >= 1;

        componentName = EditorGUILayout.TextField("Component Name", componentName);
        if (GUILayout.Button("Delete Components"))
        {
            DeleteComponentFromSelectedObjects();
        }
        GUILayout.Label(LabelSeperator, EditorStyles.boldLabel);
        //GUI.enabled = true;
    }

    void CheckResources_Characters()
    {
        var chars = Resources.LoadAll<SO_Character>("Characters");
        var charNames = new List<CharacterName>();

        foreach (var character in chars)
        {
            if (charNames.Contains(character.CharacterName))
            {
                Debug.LogError($"Character name ({character.CharacterName}) duplication detected!!! ScriptableObj Name: {character.name}");
            }
            else
            {
                charNames.Add(character.CharacterName);
            }

            if (character.Class == null)
            {
                Debug.LogWarning($"Character {character.CharacterName} class is not assigned!");
            }

            if (character.SplashArt == null)
            {
                Debug.LogWarning($"Character {character.CharacterName} splash art is not assigned!");
            }

            if (character.Voice == null)
            {
                Debug.LogWarning($"Character {character.CharacterName} voice is not assigned!");
            }
        }
    }

    private enum Axis { X, Y, Z }
    private enum Alignment { Min, Max, Average }

    private void AlignOnAxisUsingBounds(Axis axis, Alignment alignment)
    {
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length == 0) return;

        // Collect bounds data
        float[] boundsValues = new float[selectedTransforms.Length];
        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            Renderer renderer = selectedTransforms[i].GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"Selected object '{selectedTransforms[i].name}' does not have a Renderer component.");
                return;
            }

            Bounds bounds = renderer.bounds;
            switch (alignment)
            {
                case Alignment.Min:
                    boundsValues[i] = GetBoundsMin(bounds, axis);
                    break;
                case Alignment.Max:
                    boundsValues[i] = GetBoundsMax(bounds, axis);
                    break;
                case Alignment.Average:
                    boundsValues[i] = GetBoundsCenter(bounds, axis);
                    break;
            }
        }

        // Compute target value
        float targetValue = 0f;
        switch (alignment)
        {
            case Alignment.Min:
                targetValue = Mathf.Min(boundsValues);
                break;
            case Alignment.Max:
                targetValue = Mathf.Max(boundsValues);
                break;
            case Alignment.Average:
                float sum = 0f;
                foreach (float val in boundsValues)
                {
                    sum += val;
                }
                targetValue = sum / boundsValues.Length;
                break;
        }

        // Adjust positions
        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            Transform t = selectedTransforms[i];
            Renderer renderer = t.GetComponentInChildren<Renderer>();
            Bounds bounds = renderer.bounds;

            // Calculate offset
            float offset = targetValue - GetBoundsValue(bounds, axis, alignment);

            // Apply offset
            Undo.RecordObject(t, "Align Renderer Bounds");
            Vector3 position = t.position;
            position = ApplyOffset(position, axis, offset);
            position.y = 0f;
            t.position = position;
            EditorUtility.SetDirty(t);
        }
    }

    private float GetBoundsMin(Bounds bounds, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return bounds.min.x;
            case Axis.Y: return bounds.min.y;
            case Axis.Z: return bounds.min.z;
            default: return 0f;
        }
    }

    private float GetBoundsMax(Bounds bounds, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return bounds.max.x;
            case Axis.Y: return bounds.max.y;
            case Axis.Z: return bounds.max.z;
            default: return 0f;
        }
    }

    private float GetBoundsCenter(Bounds bounds, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return bounds.center.x;
            case Axis.Y: return bounds.center.y;
            case Axis.Z: return bounds.center.z;
            default: return 0f;
        }
    }

    private float GetBoundsValue(Bounds bounds, Axis axis, Alignment alignment)
    {
        switch (alignment)
        {
            case Alignment.Min: return GetBoundsMin(bounds, axis);
            case Alignment.Max: return GetBoundsMax(bounds, axis);
            case Alignment.Average: return GetBoundsCenter(bounds, axis);
            default: return 0f;
        }
    }

    private Vector3 ApplyOffset(Vector3 position, Axis axis, float offset)
    {
        switch (axis)
        {
            case Axis.X:
                position.x += offset;
                break;
            case Axis.Y:
                position.y += offset;
                break;
            case Axis.Z:
                position.z += offset;
                break;
        }
        return position;
    }

    private void ArrangeSelectedObjects()
    {
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        // Ensure columns and rows are at least 1
        numColumns = Mathf.Max(1, numColumns);
        numRows = Mathf.Max(1, numRows);

        int totalCells = numColumns * numRows;
        int objectsToArrange = Mathf.Min(selectedTransforms.Length, totalCells);

        // Get bounds and sizes
        Bounds[] boundsArray = new Bounds[objectsToArrange];
        Vector3[] sizes = new Vector3[objectsToArrange];

        for (int i = 0; i < objectsToArrange; i++)
        {
            Renderer renderer = selectedTransforms[i].GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"Selected object '{selectedTransforms[i].name}' does not have a Renderer component.");
                return;
            }
            boundsArray[i] = renderer.bounds;
            sizes[i] = boundsArray[i].size;
        }

        // Calculate max width and depth among all objects
        float maxWidth = 0f;
        float maxDepth = 0f;

        for (int i = 0; i < objectsToArrange; i++)
        {
            maxWidth = Mathf.Max(maxWidth, sizes[i].x);
            maxDepth = Mathf.Max(maxDepth, sizes[i].z);
        }

        // Starting position (using the first selected object's position)
        Vector3 startPosition = selectedTransforms[0].position;

        // Arrange objects
        for (int i = 0; i < objectsToArrange; i++)
        {
            int row = i / numColumns;
            int column = i % numColumns;

            // Calculate position offsets
            float offsetX = (maxWidth + horizontalSpacing) * column;
            float offsetZ = -(maxDepth + verticalSpacing) * row;

            // New target position
            Vector3 targetPosition = startPosition + new Vector3(offsetX, 0f, -offsetZ);

            // Adjust position based on renderer bounds
            Renderer renderer = selectedTransforms[i].GetComponentInChildren<Renderer>();
            Bounds bounds = renderer.bounds;

            // Calculate the offset between the object's position and its bounds center
            Vector3 boundsOffset = bounds.center - selectedTransforms[i].position;

            // Adjust target position to align bounding boxes
            Vector3 adjustedPosition = targetPosition - boundsOffset;
            adjustedPosition.y = 0f;

            Undo.RecordObject(selectedTransforms[i], "Grid Arrange");
            selectedTransforms[i].position = adjustedPosition;
            EditorUtility.SetDirty(selectedTransforms[i]);
        }

        // Warning if there are more objects than grid cells
        if (selectedTransforms.Length > totalCells)
        {
            Debug.LogWarning($"More objects selected ({selectedTransforms.Length}) than grid cells ({totalCells}). Only the first {totalCells} objects were arranged.");
        }
    }

    private void DistributeSelectedObjectsEvenly()
    {
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length < 2)
        {
            Debug.LogWarning("Select at least two objects to distribute evenly.");
            return;
        }

        // Collect bounds and positions
        List<Transform> transformsList = new List<Transform>(selectedTransforms);
        List<Bounds> boundsList = new List<Bounds>();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        // Get min and max positions considering the bounding boxes
        foreach (Transform t in transformsList)
        {
            Renderer renderer = t.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"Selected object '{t.name}' does not have a Renderer component.");
                return;
            }

            Bounds bounds = renderer.bounds;
            boundsList.Add(bounds);

            minX = Mathf.Min(minX, bounds.min.x);
            maxX = Mathf.Max(maxX, bounds.max.x);
            minZ = Mathf.Min(minZ, bounds.min.z);
            maxZ = Mathf.Max(maxZ, bounds.max.z);
        }

        // Calculate total available space
        float totalSpaceX = maxX - minX;
        float totalSpaceZ = maxZ - minZ;

        int objectCount = transformsList.Count;

        // Calculate the total width and depth of all objects
        float totalWidth = 0f;
        float totalDepth = 0f;
        foreach (Bounds bounds in boundsList)
        {
            totalWidth += bounds.size.x;
            totalDepth += bounds.size.z;
        }

        // Calculate spacing between objects
        float spacingX = (totalSpaceX - totalWidth) / (objectCount - 1);
        float spacingZ = (totalSpaceZ - totalDepth) / (objectCount - 1);

        // Sort objects along X axis
        transformsList.Sort((a, b) => GetRendererBounds(a).min.x.CompareTo(GetRendererBounds(b).min.x));

        // Adjust positions along X axis
        float currentX = minX;
        for (int i = 0; i < objectCount; i++)
        {
            Transform t = transformsList[i];
            Bounds bounds = boundsList[i];

            // Calculate offset to position the object's min.x at currentX
            float offsetX = currentX - bounds.min.x;

            Undo.RecordObject(t, "Distribute Evenly");
            Vector3 position = t.position;
            position.x += offsetX;
            t.position = position;
            EditorUtility.SetDirty(t);

            // Update currentX for next object
            currentX += bounds.size.x + spacingX;
        }

        // Sort objects along Z axis
        transformsList.Sort((a, b) => GetRendererBounds(a).min.z.CompareTo(GetRendererBounds(b).min.z));

        // Adjust positions along Z axis
        float currentZ = minZ;
        for (int i = 0; i < objectCount; i++)
        {
            Transform t = transformsList[i];
            Bounds bounds = boundsList[i];

            // Calculate offset to position the object's min.z at currentZ
            float offsetZ = currentZ - bounds.min.z;

            Undo.RecordObject(t, "Distribute Evenly");
            Vector3 position = t.position;
            position.z += offsetZ;
            t.position = position;
            EditorUtility.SetDirty(t);

            // Update currentZ for next object
            currentZ += bounds.size.z + spacingZ;
        }
    }

    private Bounds GetRendererBounds(Transform t)
    {
        Renderer renderer = t.GetComponentInChildren<Renderer>();
        if (renderer != null)
            return renderer.bounds;
        else
            return new Bounds(t.position, Vector3.zero);
    }

    private void DeleteComponentFromSelectedObjects()
    {
        if (string.IsNullOrEmpty(componentName))
        {
            Debug.LogWarning("Please specify a component name.");
            return;
        }

        var selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        foreach (var obj in selectedObjects)
        {
            var component = obj.GetComponent(componentName);
            while (component != null)
            {
                EditorUtility.SetDirty(component.gameObject);
                DestroyImmediate(component);
                Debug.Log($"Deleted {componentName} from {obj.name}");
                component = obj.GetComponent(componentName);
            }
        }
    }
}