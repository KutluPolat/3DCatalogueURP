using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Decision : MonoBehaviour
{
    public string FinalDecision { get; private set; }

    [SerializeField] private UIButton decisionPrefab;

    public IEnumerator StartDesicion(params string[] localizationKeysEachDesicion)
    {
        while (transform.childCount > 0)
        {
            var gobj = transform.GetChild(0).gameObject;
            gobj.SetActive(false);
            Destroy(gobj);
            yield return new WaitForEndOfFrame();
        }

        bool isDecisionMade = false;
        FinalDecision = string.Empty;

        foreach (var key in localizationKeysEachDesicion)
        {
            var button = Instantiate(decisionPrefab, transform);
            button.UIText.OverrideText(Localization.Instance.GetLocalizedString(string.Empty, key));
            button.Button.onClick.AddListener(() =>
            {
                FinalDecision = key;
                isDecisionMade = true;
            });

            button.transform.localScale = Vector3.zero;
            button.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitUntil(() => isDecisionMade);
    }
}
