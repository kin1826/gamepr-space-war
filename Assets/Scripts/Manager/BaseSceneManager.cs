using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    public static BaseSceneManager Instance;

    protected virtual void Awake()
    {
        Instance = this;
    }

    public virtual void ContinueGame()
    {

    }

    public virtual void ShowHint(string text)
    {

    }

    public virtual void HideHint()
    {

    }
    public virtual void ShowStory()
    {

    }

    public virtual void OpenSoilderPanel()
    {

    }

    public virtual void CloseSoilderPanel()
    {

    }

}