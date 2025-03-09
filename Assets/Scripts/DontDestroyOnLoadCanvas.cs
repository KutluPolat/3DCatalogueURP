using UnityEngine;

public class DontDestroyOnLoadCanvas : MonoBehaviour
{
    private static DontDestroyOnLoadCanvas Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
