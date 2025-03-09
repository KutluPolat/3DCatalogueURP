using UnityEditor;
using UnityEngine;
using System.IO;

public class ClearSaveFiles : Editor
{
    [MenuItem("Edit/Delete Save Files", false, 1000)]
    private static void DeleteSaveFiles()
    {
        // Show a confirmation dialog
        bool shouldDelete = EditorUtility.DisplayDialog(
            "Delete Save Files",
            "Are you sure you want to delete all save files?\n\nThis action cannot be undone.",
            "Delete",
            "Cancel"
        );

        // If user chooses "Delete", proceed; otherwise, do nothing.
        if (!shouldDelete)
        {
            return;
        }

        string saveFolderPath = Application.persistentDataPath;
        if (Directory.Exists(saveFolderPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
            Debug.Log("All save files have been deleted from " + saveFolderPath);
        }
        else
        {
            Debug.LogWarning("Save folder does not exist: " + saveFolderPath);
        }
    }
}
