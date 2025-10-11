using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;

    void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;

    private void Start()
    {
        LoadScene(1);
    }

    public async void LoadScene(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        _loadingPanel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            _progressSlider.value = progress;
            _progressText.text = (int)progress * 100f + "%";

            await Task.Yield();
        }
        _loadingPanel.SetActive(false);
    }
    public async void LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        _loadingPanel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            _progressSlider.value = progress;
            _progressText.text = (int)progress * 100f + "%";

            await Task.Yield();
        }
        _loadingPanel.SetActive(false);
    }
}