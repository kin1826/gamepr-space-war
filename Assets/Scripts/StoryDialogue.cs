using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class StoryDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSet
    {
        public string setName;
        [TextArea] public string[] lines;
    }

    [Header("UI")]
    public TMP_Text dialogueText;

    [Header("Dialogue Sets")]
    public List<DialogueSet> dialogueSets = new List<DialogueSet>();

    private string[] _currentLines;
    private int _currentIndex;

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

    /// <summary>
    /// Load và hiện dialogue set theo index. Gọi từ Manager trước khi ShowPanel.
    /// </summary>
    public void LoadDialogueSet(int setIndex)
    {
        if (setIndex < 0 || setIndex >= dialogueSets.Count)
        {
            Debug.LogWarning($"[StoryDialogue] Set index {setIndex} không tồn tại!");
            return;
        }

        _currentLines = dialogueSets[setIndex].lines;
        _currentIndex = 0;
        ShowLine();
    }

    void NextDialogue(InputAction.CallbackContext ctx)
    {
        if (_currentLines == null || _currentLines.Length == 0) return;

        if (_currentIndex >= _currentLines.Length - 1)
        {
            Manager.Instance.ContinueGame();
            return;
        }

        _currentIndex++;
        ShowLine();
    }

    void ShowLine()
    {
        if (_currentLines != null && _currentLines.Length > 0)
            dialogueText.text = _currentLines[_currentIndex];
    }
}
