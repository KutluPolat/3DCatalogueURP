//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Linq;

//public class FBXMaterialReplacer : EditorWindow
//{
//    private string folderPath = "Assets/";
//    private Vector2 scrollPos;
//    private List<string> fbxPaths = new List<string>();
//    private Dictionary<string, Dictionary<string, Material>> remappedMaterials = new Dictionary<string, Dictionary<string, Material>>();

//    [MenuItem("Tools/FBX Material Replacer")]
//    static void Init()
//    {
//        FBXMaterialReplacer window = (FBXMaterialReplacer)EditorWindow.GetWindow(typeof(FBXMaterialReplacer));
//        window.Show();
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("FBX Material Replacer", EditorStyles.boldLabel);

//        // Folder selection
//        GUILayout.BeginHorizontal();
//        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
//        if (GUILayout.Button("Browse", GUILayout.Width(70)))
//        {
//            string selectedFolder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
//            if (!string.IsNullOrEmpty(selectedFolder))
//            {
//                if (selectedFolder.StartsWith(Application.dataPath))
//                {
//                    folderPath = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
//                    LoadFBXFiles();
//                }
//                else
//                {
//                    EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the Assets folder.", "OK");
//                }
//            }
//        }
//        GUILayout.EndHorizontal();

//        // Load FBX files
//        if (GUILayout.Button("Load FBX Files"))
//        {
//            LoadFBXFiles();
//        }

//        // Display FBX files and materials
//        scrollPos = GUILayout.BeginScrollView(scrollPos);

//        foreach (var fbxPath in fbxPaths)
//        {
//            GUILayout.Label(fbxPath, EditorStyles.boldLabel);

//            if (!remappedMaterials.ContainsKey(fbxPath))
//            {
//                remappedMaterials[fbxPath] = new Dictionary<string, Material>();

//                // Get source materials
//                var materials = GetMaterialsFromModel(fbxPath);
//                foreach (var matName in materials)
//                {
//                    remappedMaterials[fbxPath][matName] = null;
//                }
//            }

//            var matDict = remappedMaterials[fbxPath];

//            foreach (var matName in matDict.Keys.ToList())
//            {
//                GUILayout.BeginHorizontal();
//                GUILayout.Label(matName);
//                matDict[matName] = (Material)EditorGUILayout.ObjectField(matDict[matName], typeof(Material), false);
//                GUILayout.EndHorizontal();
//            }
//        }

//        GUILayout.EndScrollView();

//        // Apply settings
//        if (GUILayout.Button("Apply Material Changes"))
//        {
//            ApplyMaterialChanges();
//        }
//    }

//    void LoadFBXFiles()
//    {
//        fbxPaths.Clear();
//        remappedMaterials.Clear();

//        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folderPath });

//        foreach (string guid in guids)
//        {
//            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

//            if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
//            {
//                fbxPaths.Add(assetPath);
//            }
//        }
//    }

//    void ApplyMaterialChanges()
//    {
//        foreach (var fbxPath in fbxPaths)
//        {
//            var matDict = remappedMaterials[fbxPath];

//            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
//            if (modelPrefab != null)
//            {
//                string prefabPath = AssetDatabase.GetAssetPath(modelPrefab);
//                GameObject modelInstance = PrefabUtility.InstantiatePrefab(modelPrefab) as GameObject;

//                // Update materials
//                Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>();
//                foreach (Renderer renderer in renderers)
//                {
//                    Material[] materials = renderer.sharedMaterials;
//                    bool materialsChanged = false;
//                    for (int i = 0; i < materials.Length; i++)
//                    {
//                        string matName = materials[i].name;
//                        if (matDict.ContainsKey(matName) && matDict[matName] != null)
//                        {
//                            materials[i] = matDict[matName];
//                            materialsChanged = true;
//                        }
//                    }
//                    if (materialsChanged)
//                    {
//                        renderer.sharedMaterials = materials;
//                    }
//                }

//                // Replace the prefab with the updated instance
//                PrefabUtility.ReplacePrefab(modelInstance, modelPrefab, ReplacePrefabOptions.ConnectToPrefab);
//                // Destroy the temporary instance
//                DestroyImmediate(modelInstance);
//            }
//        }

//        // Refresh the AssetDatabase
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }

//    List<string> GetMaterialsFromModel(string assetPath)
//    {
//        List<string> materialNames = new List<string>();

//        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
//        if (model != null)
//        {
//            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
//            foreach (Renderer renderer in renderers)
//            {
//                foreach (var mat in renderer.sharedMaterials)
//                {
//                    if (mat != null && !materialNames.Contains(mat.name))
//                    {
//                        materialNames.Add(mat.name);
//                    }
//                }
//            }
//        }
//        return materialNames;
//    }
//}
