using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Ship,
        Player
    }

    public GameState currentState;

    [Header("Objects")]
    public GameObject ship;
    public GameObject player;

    [Header("Controllers")]
    public MonoBehaviour shipController;
    public MonoBehaviour playerController;

    [Header("Ship Cameras")]
    public CinemachineCamera shipFP;
    public CinemachineCamera shipTP;

    [Header("Player Cameras")]
    public CinemachineCamera playerFP;
    public CinemachineCamera playerTP;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        EnterShip();
    }

    public void EnterShip()
    {
        currentState = GameState.Ship;

        shipController.enabled = true;
        playerController.enabled = false;

        shipFP.Priority = 20;
        shipTP.Priority = 10;

        playerFP.Priority = 0;
        playerTP.Priority = 0;
    }

    public void EnterPlayer(Transform spawnPoint = null)
    {
        currentState = GameState.Player;

        player.SetActive(true);

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }

        shipController.enabled = false;
        playerController.enabled = true;

        shipFP.Priority = 0;
        shipTP.Priority = 0;

        playerFP.Priority = 20;
        playerTP.Priority = 10;
    }
}