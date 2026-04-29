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
        // ✅ Singleton chuẩn
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitSceneReferences();

        // 👉 chỉ enter ship nếu có ship
        if (ship != null)
            EnterShip();
        else
            EnterPlayer();
    }

    void InitSceneReferences()
    {
        ship = GameObject.FindWithTag("Ship");
        player = GameObject.FindWithTag("Player");

        if (ship != null && shipController == null)
            shipController = ship.GetComponent<MonoBehaviour>();

        if (player != null && playerController == null)
            playerController = player.GetComponent<MonoBehaviour>();
    }

    public void EnterShip()
    {
        currentState = GameState.Ship;

        if (shipController != null) shipController.enabled = true;
        if (playerController != null) playerController.enabled = false;

        if (shipFP != null) shipFP.Priority = 20;
        if (shipTP != null) shipTP.Priority = 10;

        if (playerFP != null) playerFP.Priority = 0;
        if (playerTP != null) playerTP.Priority = 0;
    }

    public void EnterPlayer(Transform spawnPoint = null)
    {
        currentState = GameState.Player;

        if (player != null)
        {
            player.SetActive(true);

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }
        }

        if (shipController != null) shipController.enabled = false;
        if (playerController != null) playerController.enabled = true;

        if (shipFP != null) shipFP.Priority = 0;
        if (shipTP != null) shipTP.Priority = 0;

        if (playerFP != null) playerFP.Priority = 20;
        if (playerTP != null) playerTP.Priority = 10;
    }
}