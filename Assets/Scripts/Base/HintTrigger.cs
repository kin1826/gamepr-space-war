using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [Tooltip("Nội dung hint hiển thị khi player bước vào vùng này")]
    public string hintText;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Manager.Instance.RegisterHint(hintText);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Manager.Instance.UnregisterHint(hintText);
    }
}
