using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum MoveDirection
    {
        Forward,
        Backward,
        Right,
        Left,
        Up,
        Down,
        Custom
    }

    public Transform player;

    [Header("Movement")]
    public float speed = 10f;
    public float rotateSpeed = 3f;
    public MoveDirection moveDirection = MoveDirection.Forward;
    public Vector3 customDirection = Vector3.forward;

    void Update()
    {
        if (player == null) return;

        // 🎯 hướng tới player
        Vector3 dir = (player.position - transform.position).normalized;

        // 🎯 xoay mượt
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * rotateSpeed
        );

        // 🚀 bay tới
        transform.position += GetMoveDirection() * speed * Time.deltaTime;
    }

    Vector3 GetMoveDirection()
    {
        return moveDirection switch
        {
            MoveDirection.Backward => -transform.forward,
            MoveDirection.Right => transform.right,
            MoveDirection.Left => -transform.right,
            MoveDirection.Up => transform.up,
            MoveDirection.Down => -transform.up,
            MoveDirection.Custom => customDirection.normalized,
            _ => transform.forward
        };
    }
}
