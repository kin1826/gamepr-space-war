# SpaceWar — Project Guide

Tài liệu này dành cho developer tiếp tục dự án. Mô tả cấu trúc code, các hệ thống đã làm, và cách mở rộng.

---

## Cấu trúc Scene

| Scene | Vai trò |
|---|---|
| `CanvasLobby` | Main menu, chọn nhân vật, nhận thưởng sau trận |
| `MainScene` | Giai đoạn 1 — điều khiển phi thuyền ngoài vũ trụ |
| `BaseScene` | Giai đoạn 2a — chiến đấu bộ binh trong tàu |
| `Map` | Giai đoạn 2b — bản đồ rộng hơn trong tàu |
| `ControlRoomScene` | Giai đoạn 3 — phòng điều khiển, boss cuối |
| `EndScene` | Màn hình kết thúc |

**Thứ tự chơi:** CanvasLobby → MainScene → BaseScene → Map → ControlRoomScene → CanvasLobby

---

## Kiến trúc Manager

### Cây kế thừa
```
Manager (base)
├── BaseManager       (BaseScene)
├── MapManager        (Map)
├── ControlRoomManager (ControlRoomScene)
└── MainSceneManager  (MainScene)
```

`Manager` chứa toàn bộ virtual methods. Mỗi scene có 1 manager con override lại theo nhu cầu.

### Các method cần override ở manager con
| Method | Khi nào gọi |
|---|---|
| `ShowStory(int)` | Hiện dialogue panel |
| `ContinueGame()` | Nhấn F để tiếp tục sau dialogue |
| `OpenSoilderPanel(int, int)` | Mở panel nói chuyện với NPC |
| `CloseSoilderPanel()` | Đóng panel NPC |
| `ShowHint(string)` / `HideHint()` | Hint text góc màn hình |
| `OnWaveCleared()` | Khi tiêu diệt hết 1 wave |
| `OnPlayerDied()` | Khi player chết |
| `OnPauseGame()` / `OnResumeGame()` | Pause / resume |

### Gọi từ gameplay
```csharp
Manager.Instance.ShowStory(0);
Manager.Instance.ContinueGame();
Manager.Instance.OnWaveCleared();
Manager.Instance.OnPlayerDied();
```

---

## Hệ thống UI Panel

### UIPanelFader
Gắn lên mọi panel cần hiệu ứng fade. Dùng `CanvasGroup`.

```csharp
panel.GetComponent<UIPanelFader>().ShowPanel(); // fade in
panel.GetComponent<UIPanelFader>().HidePanel(); // fade out
```

**Lưu ý quan trọng:**
- Panel phải **active = true** trong Hierarchy
- Các manager tự `SetActive(false)` trong `Start()` rồi dùng `ShowPanel()` khi cần
- KHÔNG dùng `SetActive(false)` để ẩn panel sau khi đã setup — dùng `HidePanel()`

### ButtonController
Gắn lên button, dùng cho Home / Replay / Resume.

```csharp
OnHomeClick()   // load sceneName, cleanup cursor + GameManager
OnReplayClick() // reload scene hiện tại
OnResumeClick() // gọi Manager.TogglePause()
```

---

## Hệ thống Audio

### AudioManager
Per-scene, gắn cùng GameObject với manager. Tự tạo `AudioSource` và `AudioListener`.

**Nhạc nền:**
```csharp
AudioManager.Instance.PlayTrack(0);    // fade in track index 0
AudioManager.Instance.SwitchTrack(1); // fade out → fade in track index 1
AudioManager.Instance.StopMusic();    // fade out
```

**SFX:** Khai báo trong Inspector (`sfxClips` list — mỗi entry có Name + Clip).
```csharp
AudioManager.Instance.PlaySFX("DoorOpen");
AudioManager.Instance.PlaySFX(audioClip);
```

**Convention nhạc:** index 0 = nhạc thường, index 1 = boss/căng thẳng.

---

## Hệ thống Wave Enemy

### Cấu trúc
```
WaveRoom (GameObject)
└── EnemySpawnZone[] (waves)
```

### WaveRoom
Điều phối các zone tuần tự. Chỉ gọi `Manager.OnWaveCleared()` khi **tất cả** zone trong danh sách xong.

### EnemySpawnZone
- `autoStart = true`: tự chạy khi Start (dùng khi không có WaveRoom)
- `autoStart = false`: chờ WaveRoom gọi `StartWave()`
- Tự động detect standalone (không có WaveRoom) và gọi Manager trực tiếp

### Thêm wave mới
1. Tạo GameObject con trong WaveRoom
2. Gắn `EnemySpawnZone`, set `autoStart = false`
3. Kéo vào list `waves` của WaveRoom theo thứ tự

---

## Hệ thống Reward

### Luồng hoàn chỉnh
```
BaseManager.Awake()
  → tạo SessionReward (DontDestroyOnLoad)

Enemy chết
  → EnemyReward.GiveReward()
  → SessionReward.Add(gold, xp)
    XP = maxHealth / 10
    Gold = Random(minGold, maxGold)

Win (ControlRoomManager)
  → PlayerPrefs["ShowReward"] = 1
  → FadeManager.LoadScene("CanvasLobby")

CanvasLobby
  → LobbyManager.Start()
  → đọc SessionReward → set earnedGold/earnedXP
  → SessionReward.Consume() (destroy)
  → ShowReward() → RewardPanelUI.Preview()

Player nhấn nút Nhận
  → RewardPanelUI.OnClaimClick()
  → cộng vào PlayerData → SaveManager.Save()
  → animate đếm số lên UI
```

### EnemyReward
Gắn lên prefab enemy. Tự lấy XP từ `EnemyHealth.maxHealth / 10`.

```
[Inspector]
minGold = 1, maxGold = 3    ← skeleton
minGold = 5, maxGold = 8    ← boss
```

### PlayerData (JSON)
Lưu tại: `%AppData%/../LocalLow/DefaultCompany/SpaceWar/playerdata.json`

| Field | Mô tả |
|---|---|
| `level` | Level hiện tại |
| `currentXP` | XP hiện tại trong level |
| `xpToNextLevel` | XP cần để lên level (tăng 30% mỗi level) |
| `gold` | Vàng tích lũy |
| `diamond` | Kim cương (chưa có nguồn thu, dùng sau) |

---

## Hệ thống Save/Load

```csharp
PlayerData data = SaveManager.Load(); // load từ JSON, trả new PlayerData nếu chưa có file
data.gold += 100;
SaveManager.Save(data);               // ghi xuống JSON
SaveManager.Delete();                 // xóa file (reset)
```

---

## FadeManager

DontDestroyOnLoad. Xử lý fade đen khi chuyển scene.

```csharp
FadeManager.Instance.LoadScene("CanvasLobby");
```

Tự fade out → load → fade in.

---

## Những thứ chưa làm / cần làm tiếp

| Hạng mục | Ghi chú |
|---|---|
| Kim cương | Field đã có trong PlayerData, chưa có nguồn thu |
| EarnedXP/Gold từ ControlRoom | Hiện tại chỉ dùng SessionReward tổng hợp từ enemy, chưa có bonus mission |
| Hệ thống nhân vật lobby | FigureController có sẵn, chưa kết nối với selection UI |
| AudioManager không nghe được | Cần debug — có thể chưa gắn component vào scene hoặc chưa kéo clip vào list |
| EndScene | Scene đã có nhưng chưa có nội dung |
| Boss Wolf | Script `Wolfbossattack3.cs` trong `PhanCuaQuan/` — đang phát triển riêng |

---

## Quy ước code

- Manager gọi nhau qua `Manager.Instance.*` (không dùng Find)
- Panel dùng `UIPanelFader.ShowPanel()` / `HidePanel()`, không dùng `SetActive` sau init
- Audio luôn qua `AudioManager.Instance`, không dùng AudioSource trực tiếp
- Scene transition luôn qua `FadeManager.Instance.LoadScene()`
- Không gọi `Manager.OnWaveCleared()` từ `EnemySpawnZone` nếu đang trong `WaveRoom`
