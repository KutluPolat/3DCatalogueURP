//#define VIEWPORT
//#define DEBUG_RAYCAST

public abstract class AllPurpose
{
#if DEBUG_RAYCAST

    public static void RaycastDebug(Vector3 rayOrigin, Vector3 rayDir, Vector3 hitPoint, Color color, float duration = 3f)
    {
        Debug.DrawRay(rayOrigin, rayDir, color, duration);
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive.GetComponent<Renderer>().material.color = color;
        primitive.transform.position = hitPoint;
        MonoBehaviour.Destroy(primitive, duration);
    }

#endif

    #if VIEWPORT

    private static Vector3 VIEWPORT_POSITION;

    public static Vector3 GetClampedPositionOnViewport(Vector3 currentPos, float xClamp = 0.05f, float yClamp = 0.05f)
    {
        if (IsInRange(xClamp, 0, 0.5f) == false && IsInRange(yClamp, 0, 0.5f) == false)
        {
            Debug.LogError("Possible clamp range is [0, 0.5f], please enter new value in this range.");
            Debug.Break();
        }

        VIEWPORT_POSITION = GameManager.Instance.ViewportCamera.WorldToViewportPoint(currentPos);
        VIEWPORT_POSITION.x = Mathf.Clamp(VIEWPORT_POSITION.x, 0 + xClamp, 1 - xClamp);
        VIEWPORT_POSITION.y = Mathf.Clamp(VIEWPORT_POSITION.y, 0 + yClamp, 1 - yClamp);

        return GameManager.Instance.ViewportCamera.ViewportToWorldPoint(VIEWPORT_POSITION);
    }

    public static Vector3 GetRandWorldPosInViewport()
    {
        VIEWPORT_POSITION.x = Random.Range(0.1f, 0.9f);
        VIEWPORT_POSITION.y = Random.Range(0.1f, 0.9f);

        return GameManager.Instance.ViewportCamera.ViewportToWorldPoint(VIEWPORT_POSITION);
    }

    #endif
}
