using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneLoadManager.Instance.LoadScene(sceneIndex);
    }
}
