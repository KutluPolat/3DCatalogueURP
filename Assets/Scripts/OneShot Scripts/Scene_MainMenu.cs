using DG.Tweening;
using KPFramework;
using System.Collections;
using UnityEngine;

public class Scene_MainMenu : MenuLoader
{
    public bool IsLoadingAnimStarted { get; private set; }
    public Transform WarpGateContainer, WarpGateModel, SpaceShip, ParentMovingObjects;
    public ParticleSystem WarpGateParticle;
    [SerializeField] private Transform[] notMovingObjs;
    private Vector3[] nmoOffsets;
    public float currentSpeed { get; private set; }
    private readonly float maxSpeed = 100f;

    private void Awake()
    {
        foreach (Transform child in ParentMovingObjects)
        {
            child.gameObject.AddComponent<Scene_MainMenu_MovingObject>().Init(this);
        }

        nmoOffsets = new Vector3[notMovingObjs.Length];
        for (int i = 0; i < notMovingObjs.Length; i++)
        {
            nmoOffsets[i] = notMovingObjs[i].position - SpaceShip.position;
        }
    }

    private void Start()
    {
        AudioManager.Instance.PlayEnvironment(AudioNames.Environment_CreepyWhiteNoise, true);
    }

    private void OnDisable()
    {
        AudioManager.Instance.StopSpecificAudioFirst(AudioType.Environment, AudioNames.Environment_CreepyWhiteNoise, 0f);
        WarpGateModel.DOKill();
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopSpecificAudioFirst(AudioType.Environment, AudioNames.Environment_CreepyWhiteNoise, 0f);

        if (WarpGateModel != null)
            WarpGateModel.DOKill();
    }

    private void Update()
    {
        if (!IsLoadingAnimStarted)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime);

            for (int i = 0; i < notMovingObjs.Length; i++)
            {
                notMovingObjs[i].position = SpaceShip.position + nmoOffsets[i];
            }
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * 0.5f, Time.deltaTime * 3f);
        }

        SpaceShip.position += currentSpeed * Time.deltaTime * Vector3.forward;
    }

    public override IEnumerator PlayLoadGameAnimation()
    {
        var spaceEffects = GetComponent<SpaceEffects>();

        Events.InvokeEvent(EventName.TriggerUIAction, new UIAction(UIPanel.Unselected, true, false));
        IsLoadingAnimStarted = true;
        //StartHyperBoosters

        if (spaceEffects != null)
            yield return spaceEffects.StartHyperDrive();

        yield return new WaitForSeconds(0.5f);

        MainCamera.vcam.SetFollowTarget(null);
        MainCamera.vcam.SetAimTarget(null);

        // RotateWarpGate
        WarpGateModel.DOLocalRotate(Vector3.up * 1800f, 2f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);

        // PlayWarpGateParticle
        WarpGateParticle.Play();

        if (spaceEffects != null)
            yield return spaceEffects.CompleteHyperDrive(WarpGateContainer, SpaceShip);

        yield return new WaitForSeconds(0.25f);
    }
}
