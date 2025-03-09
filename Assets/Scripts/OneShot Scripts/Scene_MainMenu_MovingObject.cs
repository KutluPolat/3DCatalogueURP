using DG.Tweening;
using UnityEngine;

public class Scene_MainMenu_MovingObject : MonoBehaviour
{
    private Scene_MainMenu mainMenu;
    private Vector3 startOffset;
    private Vector3 startScale;
    private bool safeToGo;

    public void Init(Scene_MainMenu menu)
    {
        mainMenu = menu;

        startOffset = transform.position - mainMenu.SpaceShip.position;
        startScale = transform.localScale;
    }

    private void OnBecameInvisible()
    {
        Respawn();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void Respawn()
    {
        if (this == null || enabled == false || transform == null || mainMenu.IsLoadingAnimStarted)
            return;

        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(startScale, 10f).SetEase(Ease.Linear);
        transform.position = mainMenu.SpaceShip.position + startOffset + Vector3.forward * Random.Range(3000f, 4000f);
    }
}
