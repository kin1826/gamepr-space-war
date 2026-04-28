using UnityEngine;

public class DoorComController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;

    public float openDistance = 6f;
    public float speed = 3f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;

    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpen = false;

    void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftOpenPos = leftClosedPos + Vector3.back * openDistance;
        rightOpenPos = rightClosedPos + Vector3.forward * openDistance;
    }

    void Update()
    {
        Vector3 targetLeft = isOpen ? leftOpenPos : leftClosedPos;
        Vector3 targetRight = isOpen ? rightOpenPos : rightClosedPos;

        leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, targetLeft, Time.deltaTime * speed);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, targetRight, Time.deltaTime * speed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        Debug.Log(isOpen ? "Door Opening" : "Door Closing");
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}