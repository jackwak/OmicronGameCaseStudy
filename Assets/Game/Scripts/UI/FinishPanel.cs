using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishPanel : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _stageText;
    [SerializeField] private CanvasGroup _contentCanvasGroup;

    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;

    void OnEnable()
    {
        EventManager.Instance.GameWin += OpenWinPanel;
        EventManager.Instance.GameLose += OpenLosePanel;
    }

    void OnDisable()
    {
        EventManager.Instance.GameWin -= OpenWinPanel;
        EventManager.Instance.GameLose -= OpenLosePanel;
    }

    private void OpenPanel()
    {
        _backgroundImage.gameObject.SetActive(true);
        _contentCanvasGroup.gameObject.SetActive(true);
    }

    private void OpenWinPanel()
    {
        OpenPanel();
        _stageText.text = "Stage Completed";
        _backgroundImage.color = _winColor;
        _backgroundImage.transform.DOScale(Vector3.one, .3f).From(Vector3.zero).SetEase(Ease.Linear);
        _contentCanvasGroup.DOFade(1, .3f).From(0).SetEase(Ease.Linear);
    }

    private void OpenLosePanel()
    {
        OpenPanel();
        _stageText.text = "Stage Lost";
        _backgroundImage.color = _loseColor;
        _backgroundImage.transform.DOScale(Vector3.one, .3f).From(Vector3.zero).SetEase(Ease.Linear);
        _contentCanvasGroup.DOFade(1, .3f).From(0).SetEase(Ease.Linear);
    }
}
