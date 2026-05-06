using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public CharacterController controller;

    public float bobSpeed = 8f;
    public float bobAmount = 0.03f;

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        Vector3 velocity = controller.velocity;

        bool isMoving =
            Mathf.Abs(velocity.x) > 0.1f ||
            Mathf.Abs(velocity.z) > 0.1f;

        if (isMoving && controller.isGrounded)
        {
            timer += Time.deltaTime * bobSpeed;

            transform.localPosition = startPos + new Vector3(
                0,
                Mathf.Sin(timer) * bobAmount,
                0
            );
        }
        else
        {
            timer = 0;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * 5f
            );
        }
    }
}