using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Điều phối nhiều đợt quái tuần tự trong 1 phòng.
/// Đợt trước xong mới spawn đợt tiếp theo.
/// Hết tất cả đợt → mở cửa (enable các Collider trigger).
/// </summary>
public class WaveRoom : MonoBehaviour
{
    [Tooltip("Các đợt quái theo thứ tự. Đặt autoStart = false trên từng zone.")]
    public List<EnemySpawnZone> waves = new List<EnemySpawnZone>();

    [Tooltip("Các Collider trigger của cửa cần bật khi hết tất cả đợt.")]
    public List<Collider> doorsToUnlock = new List<Collider>();

    [Tooltip("Các đèn chỉ thị trạng thái cửa (đỏ = khoá, xanh = mở).")]
    public List<DoorIndicator> doorIndicators = new List<DoorIndicator>();

    private int _currentWave = -1;

    void Start()
    {
        foreach (var door in doorsToUnlock)
            if (door) door.enabled = false;

        foreach (var indicator in doorIndicators)
            if (indicator) indicator.SetState(false);

        StartNextWave();
    }

    void StartNextWave()
    {
        _currentWave++;

        if (_currentWave >= waves.Count)
        {
            UnlockDoors();
            return;
        }

        var zone = waves[_currentWave];
        if (zone == null)
        {
            StartNextWave();
            return;
        }

        zone.onZoneCleared += OnZoneCleared;
        zone.StartWave();

        Debug.Log($"[WaveRoom] Bắt đầu đợt {_currentWave + 1}/{waves.Count}: {zone.name}");
    }

    void OnZoneCleared()
    {
        waves[_currentWave].onZoneCleared -= OnZoneCleared;
        Debug.Log($"[WaveRoom] Đợt {_currentWave + 1}/{waves.Count} xong. Chuyển đợt tiếp...");
        StartNextWave();
    }

    void UnlockDoors()
    {
        Debug.Log($"[WaveRoom] Tất cả đợt xong! Mở cửa.");

        foreach (var door in doorsToUnlock)
            if (door) door.enabled = true;

        foreach (var indicator in doorIndicators)
            if (indicator) indicator.SetState(true);

        Manager.Instance?.OnWaveCleared();
    }
}
