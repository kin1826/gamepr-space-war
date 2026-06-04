using UnityEngine;
using UnityEngine.SceneManagement;
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
    private GameObject ship;
    private GameObject player;

    [Header("Controllers")]
    private PlayerController shipController;
    private PlayerControl playerController;

    [Header("Ship Cameras")]
    private CinemachineCamera shipFP;
    private CinemachineCamera shipTP;

    [Header("Player Cameras")]
    private CinemachineCamera playerFP;
    private CinemachineCamera playerTP;

    void Awake()
    {
        // ✅ Singleton


        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        InitSceneReferences();

        if (ship != null)
            EnterShip();
        else
            EnterPlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitSceneReferences();

        if (ship != null)
            EnterShip();
        else
            EnterPlayer();
    }

    void InitSceneReferences()
    {
        // 🎯 OBJECTS
        ship = GameObject.FindWithTag("Ship");
        player = GameObject.FindWithTag("Player");

        // 🎯 CONTROLLERS
        if (ship != null)
            shipController = ship.GetComponent<PlayerController>();

        if (player != null)
            playerController = player.GetComponent<PlayerControl>();

        // 🎯 CAMERAS
        shipFP = FindCamera("VCam_Inside");
        shipTP = FindCamera("VCam_Outside");

        playerFP = FindCamera("PlayerCam_Inside");
        playerTP = FindCamera("PlayerCam_Outside");
    }

    CinemachineCamera FindCamera(string objName)
    {
        GameObject obj = GameObject.Find(objName);

        if (obj == null)
            return null;

        return obj.GetComponent<CinemachineCamera>();
    }

    public void EnterShip()
    {
        currentState = GameState.Ship;

        if (shipController != null)
            shipController.enabled = true;

        if (playerController != null)
            playerController.enabled = false;

        SetCameraPriority(shipFP, 20);
        SetCameraPriority(shipTP, 10);

        SetCameraPriority(playerFP, 0);
        SetCameraPriority(playerTP, 0);
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

        if (shipController != null)
            shipController.enabled = false;

        if (playerController != null)
            playerController.enabled = true;

        SetCameraPriority(shipFP, 0);
        SetCameraPriority(shipTP, 0);

        SetCameraPriority(playerFP, 20);
        SetCameraPriority(playerTP, 10);
    }

    void SetCameraPriority(CinemachineCamera cam, int priority)
    {
        if (cam != null)
            cam.Priority = priority;
    }
}