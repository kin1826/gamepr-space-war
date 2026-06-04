using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;

    public float openDistance = 20f;
    public float speed = 3f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;

    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private Coroutine currentRoutine;

    private bool isOpen = false;

    void Start()
    {
        // 📌 lưu vị trí ban đầu
        leftClosedPos = doorLeft.localPosition;
        rightClosedPos = doorRight.localPosition;

        // 📌 tính vị trí mở
        leftOpenPos = leftClosedPos - doorLeft.transform.forward * openDistance;
        rightOpenPos = rightClosedPos + doorRight.transform.forward * openDistance;
    }

    public void OpenDoor()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(MoveDoor(leftOpenPos, rightOpenPos));
    }

    public void CloseDoor()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(MoveDoor(leftClosedPos, rightClosedPos));
    }

    IEnumerator MoveDoor(Vector3 leftTarget, Vector3 rightTarget)
    {
        while (Vector3.Distance(doorLeft.localPosition, leftTarget) > 0.01f)
        {
            doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, leftTarget, Time.deltaTime * speed);
            doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, rightTarget, Time.deltaTime * speed);

            yield return null;
        }

        // đảm bảo đúng vị trí cuối
        doorLeft.localPosition = leftTarget;
        doorRight.localPosition = rightTarget;
    }

    public void ToggleDoor()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        AudioManager.Instance.PlaySFX("DoorToggle"); // Phát âm thanh mở/đóng cửa (giả sử clip đầu tiên trong sfxClips là âm thanh này)
        if (isOpen)
        {
            currentRoutine = StartCoroutine(MoveDoor(leftClosedPos, rightClosedPos));
            isOpen = false;
        }
        else
        {
            currentRoutine = StartCoroutine(MoveDoor(leftOpenPos, rightOpenPos));
            isOpen = true;
        }
    }
}
