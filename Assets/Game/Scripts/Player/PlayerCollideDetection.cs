using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerCollideDetection : MonoBehaviour
{
    [Header(" References ")]
    [SerializeField] private Transform _shiledTransform;
    
    [Header(" Settings ")]
    [SerializeField] private float  _shiledEndDuration = 2f;
    private bool _isShieldActive = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HexagonStack hexagonStack))
        {
            if (_isShieldActive)
            {

            }
            else
            {
                EventManager.Instance.OnCollideWithOctagon();

                _isShieldActive = true;
                _shiledTransform.gameObject.SetActive(true);
                _shiledTransform.DOScale(_shiledTransform.localScale + Vector3.one * .2f, .6f).From(_shiledTransform.localScale).SetLoops(-1, LoopType.Yoyo);
                StartCoroutine(StartShieldEndDuration());
            }    
        }
    }

    private IEnumerator StartShieldEndDuration()
    {
        yield return new WaitForSeconds(_shiledEndDuration);
        _isShieldActive = false;
    }
}
