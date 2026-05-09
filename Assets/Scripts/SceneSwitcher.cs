using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private string sceneName = "Map";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FadeManager.Instance.LoadScene(sceneName);
    }
}