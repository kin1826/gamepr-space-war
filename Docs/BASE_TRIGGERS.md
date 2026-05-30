# Base Components & Triggers

---

## Triggers

### HintTrigger.cs
Hiện hint khi player vào vùng, về default hint khi ra.

```
Hint Text = "Nhấn [F] để tương tác"
```

Gắn lên GameObject có Collider (Is Trigger = true), tag object không quan trọng.

---

### SoldierTrigger.cs
Mở soldier panel + camera khi player vào. Chỉ trigger **1 lần**.

```
Panel Index = 0    ← index trong MapManager.soidlerPanels
Cam Index   = 1    ← index trong MapManager.machineCams
```

---

### DoorTrigger.cs
Toggle cửa khi Player / Ship / Enemy vào trigger.

Gắn lên: collider trước cửa.
Kéo `DoorController` vào field `door`.

---

### EnterPlayerTrigger.cs
Khi Ship vào trigger → `GameManager.EnterPlayer(spawnPoint)`.
`spawnPoint` optional — nếu null thì player spawn tại vị trí trigger.

---

### ObjectiveTrigger.cs
Khi player vào trigger:
- `nextObjective = true` → advance sang objective tiếp theo
- `nextObjective = false` → end objective (ẩn marker)

---

### SceneSwitcher.cs / SceneLoader.cs
Load scene khi trigger:

```
Scene Name = "MapScene"
```

Dùng `FadeManager.LoadScene()` để có hiệu ứng fade.

---

## Interactive Objects

### ComputerInteraction.cs
Computer terminal cho phép mở/đóng cửa **nếu wave đã clear**.

| Field | Mô tả |
|-------|-------|
| `door` | DoorComController |
| `materialChange` | MaterialChange component để đổi màu screen |

**Logic:**
- Đứng trong range → hint động theo trạng thái cửa
- Nếu `isWaveCleared = false` → hint "Defeat all enemies"
- Nếu `isWaveCleared = true` → hint "Press F to OPEN/CLOSE"
- Nhấn F → toggle cửa

**MaterialChange:**
- `[0]` = material đỏ (wave chưa clear)
- `[1]` = material xanh (wave đã clear)

---

### MaterialChange.cs
Đổi material theo index. Gắn lên object có Renderer.

```
States:
  [0] → material đỏ
  [1] → material xanh
  [2] → material vàng
  ...
```

```csharp
materialChange.SetState(0);  // đỏ
materialChange.SetState(1);  // xanh
```

Nếu `targetRenderer` để trống → tự lấy Renderer trên cùng object.

---

## UI Components

### UIPanelFader.cs
Fade in/out panel mượt qua CanvasGroup.

```csharp
panel.GetComponent<UIPanelFader>().ShowPanel();
panel.GetComponent<UIPanelFader>().HidePanel();
```

Gắn lên: root của panel UI, cần có `CanvasGroup` component.

---

### StoryDialogue.cs
Hiện dialogue tuần tự, nhấn F để next.

```
Dialogues:
  [0] "Chào mừng đến với..."
  [1] "Nhiệm vụ của bạn là..."
  [2] "Hãy cẩn thận..."
```

---

### Loading_Manager.cs
Loading screen với slider progress (fake loading, không load async thật).

| Field | Default | Mô tả |
|-------|---------|-------|
| `sceneName` | "MainScene" | Scene cần load |
| `loadingSpeed` | 0.5 | Tốc độ slider fill |

**Quan trọng:** Gắn lên object **khác** với loadingPanel để tránh bị disable cùng panel.

---

## Procedural / Utility

### TileGridSpawner.cs
Sinh tile grid (nền sàn).

```
Tile Prefab = [tile prefab]
Width = 10
Height = 10
Tile Size = 1
```

### FanRotate.cs
Quay object liên tục.
```
Speed = 300f  ← độ/giây
```

### RagdollManager.cs
Kích hoạt ragdoll khi enemy chết.
- Start: tất cả Rigidbody → isKinematic = true
- `TriggerRagdoll()`: tất cả Rigidbody → isKinematic = false

Gắn cùng object với EnemyHealth.

---

## Objective Marker System

### ObjectiveMarkerSystem.cs
Hiện marker objective trên màn hình (on-screen) và mũi tên ở rìa màn hình (off-screen).

```
Objectives:
  [0] Name="Tìm cửa ra"  Target=[Transform]
  [1] Name="Tiêu diệt boss"  Target=[Transform]
```

```csharp
system.SetObjective(1);    // chuyển sang objective index 1
system.NextObjective();    // objective tiếp theo
system.EndObjective();     // ẩn marker
```

Marker tự scale nhỏ lại khi gần objective.
