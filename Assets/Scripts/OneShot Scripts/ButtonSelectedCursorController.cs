//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
//using UnityEngine.UI;

//public class ButtonSelectedCursorController : MonoBehaviour
//{
//    //WARNINGS
//    //add canvas image that you like and assign this script and done!
//    //on your event system you have to assign manually first selected button.
//    //if you have invisible button on your scene just setactive false it otherwise it will navigate to that button
//    private RectTransform currentObj;
//    private Image img;
//    private void Awake()
//    {
//        img = GetComponent<Image>();
//    }
//    public void Start()
//    {
//        if (Gamepad.current != null)
//        {
//            ToggleCursor(true);
//            return;
//        }
//        ToggleCursor(false);
//    }
//    private void ToggleCursor(bool status)
//    {
//        img.enabled = status;

//    }
//    private void Update()
//    {
//        CheckGamepad();
//        if (EventSystem.current.currentSelectedGameObject != currentObj && EventSystem.current.currentSelectedGameObject != null)
//        {

//            currentObj = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
//            RePositionCursor();
//        }
//        if (EventSystem.current.currentSelectedGameObject == null)
//        {
//            ReSelectFirst();
//        }
//        else if (!EventSystem.current.currentSelectedGameObject.activeInHierarchy)
//        {
//            ReSelectFirst();
//        }
//    }
//    private void CheckGamepad()
//    {
//        if (Gamepad.current != null)
//        {
//            ToggleCursor(true);
//        }
//        else
//        {
//            ToggleCursor(false);
//        }
//    }
//    private void ReSelectFirst()
//    {
//        currentObj = EventSystem.current.firstSelectedGameObject.GetComponent<RectTransform>();
//        EventSystem.current.SetSelectedGameObject(currentObj.gameObject);
//        RePositionCursor();
//    }
//    private void RePositionCursor()
//    {
//        transform.position = currentObj.transform.position - Vector3.up * (currentObj.rect.height / 2f);
//    }
//}