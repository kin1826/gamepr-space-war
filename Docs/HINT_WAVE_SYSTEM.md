# Hint System & Wave System

---

## Hint System

### Cơ chế Stack

Hệ thống hint dùng **stack ưu tiên** — hint mới nhất luôn hiện lên đầu.

```
Player vào trigger A  →  RegisterHint("Nhấn F để mở cửa")    [stack: A]
Player vào trigger B  →  RegisterHint("Nhấn E để tương tác") [stack: A, B] → hiện B
Player ra trigger B   →  UnregisterHint("Nhấn E tương tác")  [stack: A]   → về lại A
Player ra trigger A   →  UnregisterHint("Nhấn F để mở cửa") [stack: rỗng] → hiện default
```

### HintTrigger Component

Gắn `HintTrigger.cs` lên bất kỳ GameObject có Collider(Trigger):

```
Hint Text = "Nhấn [F] để tương tác"
```

Tự động gọi `RegisterHint` khi enter, `UnregisterHint` khi exit.

### Default Hints

Khai báo trong Inspector của Manager:
```
Default Hints:
  [0] "Dùng [WASD] để di chuyển"
  [1] "Tiêu diệt hết kẻ thù!"
  [2] "Tìm đường thoát ra ngoài"
```

Chuyển default hint từ code:
```csharp
Manager.Instance.SetDefaultHint(1);  // → "Tiêu diệt hết kẻ thù!"
Manager.Instance.NextDefaultHint();  // → đổi sang hint tiếp theo
```

### Hint động (thay đổi từng frame)

Với hint động (ví dụ ComputerInteraction), gọi `ShowHint()` trực tiếp trong `Update()`. Khi exit, gọi `Manager.Instance.ShowDefaultHint()` thay vì `HideHint()`.

```csharp
void Update()
{
    if (!playerInRange) return;
    Manager.Instance.ShowHint(door.IsOpen() ? "F - ĐÓNG" : "F - MỞ");
}

void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        playerInRange = false;
        Manager.Instance.ShowDefaultHading(); // ← về default
    }
}
```

---

## Wave System

### Flow

```
EnemySpawnZone.StartWave()
    ↓ spawn từng enemy, interval giữa mỗi con
    ↓ mỗi enemy được subscribe onDeath event
Enemy bị giết → EnemyHealth.EnemyDeath() → onDeath?.Invoke()
    ↓
EnemySpawnZone.HandleEnemyDied(go)
    ↓ xóa khỏi _aliveEnemies list
    ↓ nếu Count == 0:
NotifyWaveCleared() → Manager.Instance.OnWaveCleared()
    ↓
BaseManager.OnWaveCleared() → isWaveCleared = true
```

### EnemySpawnZone — Setup

| Field | Mô tả |
|-------|-------|
| `enemyPrefab` | Prefab enemy cần spawn |
| `maxEnemies` | Số lượng tối đa 1 wave |
| `spawnInterval` | Giây giữa 2 lần spawn |
| `zoneSize` | Kích thước vùng spawn (Gizmo cam màu da cam) |

**Gắn vào:** Empty GameObject, chỉnh `Zone Size` trong Inspector.

**Bắt đầu wave:**
- Mặc định tự gọi `StartWave()` trong `Start()`
- Hoặc gọi từ code: `spawnZone.StartWave()`
- Hủy wave: `spawnZone.CancelWave()`

### Enemy cần có

- `EnemyHealth` component (có `onDeath` event)
- `RagdollManager` component

Nếu enemy thiếu `EnemyHealth`, wave sẽ không track được và `OnWaveCleared` không bao giờ gọi.

### isWaveCleared

Set trong `BaseManager.OnWaveCleared()` và `MapManager.OnWaveCleared()`.

Dùng để kiểm tra điều kiện mở cửa:
```csharp
// ComputerInteraction.Update()
bool waveCleared = BaseManager.Instance != null && BaseManager.Instance.isWaveCleared;
if (waveCleared) { /* cho mở cửa */ }
```
