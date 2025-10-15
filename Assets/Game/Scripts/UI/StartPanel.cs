using DG.Tweening;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private Transform _tapToStartText;

    private void OnEnable()
    {
        EventManager.Instance.EnterReadyState += OpenText;
        EventManager.Instance.EnterGameState += CloseText;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterReadyState -= OpenText;
        EventManager.Instance.EnterGameState -= CloseText;
    }

    private void OpenText()
    {
        _tapToStartText.DOScale(Vector3.one * 1.2f, 1f).From(Vector3.one).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        _tapToStartText.gameObject.SetActive(true);
    }

    private void CloseText()
    {
        DOTween.KillAll();
        _tapToStartText.gameObject.SetActive(false);
    }
}
