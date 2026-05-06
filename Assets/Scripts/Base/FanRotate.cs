using UnityEngine;

public class FanRotate : MonoBehaviour
{
    public float speed = 300f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
}