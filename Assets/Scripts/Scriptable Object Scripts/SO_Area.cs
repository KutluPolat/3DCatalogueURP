using KPFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "New Area", menuName = "Kutlu/Area")]
public class SO_Area : ScriptableObject
{
    public TextAsset LocalizationResource => localizationResource;
    public string AreaTitle => areaTitle;
    public string AreaTitleLocalized => Localization.Instance.GetLocalizedString(areaTitle, areaTitleKey); // TO-DO bunu yazdigimiz texti de LocalizeText methodu ile yazmamiz lazim
    public string AreaTitleKey => areaTitleKey;
    public Chapter Chapter => chapter;

    [SerializeField] private TextAsset localizationResource;
    [SerializeField] private string areaTitle;
    [SerializeField] private string areaTitleKey;
    [SerializeField] private Chapter chapter;
}
