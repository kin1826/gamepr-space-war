using UnityEngine;
using Unity.Cinemachine;

public class CameraSpeedOffset : MonoBehaviour
{
    public CinemachineCamera cam;
    public PlayerController player;

    public float minDistance = 5f;
    public float maxDistance = 10f;
    public float smooth = 2f;

    private CinemachineThirdPersonFollow follow;
    private float currentDistance;

    void Start()
    {
        // 👉 LẤY COMPONENT ĐÚNG
        follow = cam.GetComponent<CinemachineThirdPersonFollow>();

        if (follow == null)
        {
            Debug.LogError("Chưa có ThirdPersonFollow trên camera!");
            return;
        }

        currentDistance = minDistance;
    }

    void Update()
    {
        if (follow == null) return;

        float speedPercent = player.GetSpeedPercent();
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, speedPercent);

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smooth);

        // 👉 SET KHOẢNG CÁCH
        follow.CameraDistance = currentDistance;
    }
}