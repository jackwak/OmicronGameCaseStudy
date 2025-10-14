using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FinishPanel : MonoBehaviour
{
    [SerializeField] private Transform _backgroundTransform;
    [SerializeField] private CanvasGroup _contentCanvasGroup;

    void OnEnable()
    {
        EventManager.Instance.EnterFinishState += OpenFinishPanel;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterFinishState -= OpenFinishPanel;
    }

    private void OpenFinishPanel()
    {
        _backgroundTransform.gameObject.SetActive(true);
        _contentCanvasGroup.gameObject.SetActive(true);

        _backgroundTransform.DOScale(Vector3.one, .3f).From(Vector3.zero).SetEase(Ease.Linear);
        _contentCanvasGroup.DOFade(1, .3f).From(0).SetEase(Ease.Linear);
    }
}
