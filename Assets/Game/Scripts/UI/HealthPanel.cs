using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour
{
    [SerializeField] private Transform[] _heartTransforms;
    private int _unvisibaleHeartIndex = 0;
    void OnEnable()
    {
        EventManager.Instance.CollideWithOctagon += UnVisibleHealth;
    }

    void OnDisable()
    {
        EventManager.Instance.CollideWithOctagon -= UnVisibleHealth;
    }

    private void UnVisibleHealth()
    {
        if (_unvisibaleHeartIndex < _heartTransforms.Length && _heartTransforms[_unvisibaleHeartIndex] != null)
        {
            _heartTransforms[_unvisibaleHeartIndex].DOScale(Vector3.zero, .2f).From(Vector3.one).SetEase(Ease.InBack);
            _unvisibaleHeartIndex++;

            if (_heartTransforms.Length <= _unvisibaleHeartIndex)
            {
                // Open Lose Panel
                EventManager.Instance.OnGameLose();
                GameManager.Instance.SwitchToFinishState();
            }
        }
    }
}
