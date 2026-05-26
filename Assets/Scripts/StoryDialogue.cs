using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class StoryDialogue : MonoBehaviour
{

    [Header("UI")]
    public TMP_Text dialogueText;

    [Header("Dialogue")]
    [TextArea]
    public string[] dialogues;

    private int currentIndex = 0;

    private InputSystem_Actions input;

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Interact.performed += NextDialogue;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= NextDialogue;

        input.Disable();
    }

    void Start()
    {
        ShowDialogue();

    }

    void NextDialogue(InputAction.CallbackContext ctx)
    {
        // 🎯 nếu đang ở câu cuối
        if (currentIndex >= dialogues.Length - 1)
        {
            Manager.Instance.ContinueGame();

            return;
        }

        currentIndex++;

        ShowDialogue();
    }

    void ShowDialogue()
    {
        dialogueText.text = dialogues[currentIndex];
    }
}