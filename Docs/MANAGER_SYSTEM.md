# Manager System

## Hierarchy

```
Manager (base, Singleton)
  ├── BaseManager
  ├── MapManager
  ├── ControlRoomManager
  └── MainSceneManager
```

`Manager.Instance` được set trong `Awake()` của class con qua `base.Awake()`.
Mỗi manager con cũng có static `Instance` riêng (vd: `MapManager.Instance`).

---

## Manager.cs — Base class

### Virtual methods (override ở class con)
| Method | Mục đích |
|--------|----------|
| `ShowHint(string text)` | Hiện hint text |
| `HideHint()` | Ẩn hint |
| `ShowStory(int index = 0)` | Hiện story panel theo index |
| `ContinueGame()` | Thoát story/panel, vào gameplay |
| `OpenSoilderPanel(int panelIndex, int camIndex)` | Mở soldier panel + đổi cam |
| `CloseSoilderPanel()` | Đóng soldier panel |
| `OnWaveCleared()` | Callback khi tiêu diệt hết wave |
| `OnAmmoChanged(int current, int extra)` | Callback thay đổi đạn |
| `OnPlayerHealthChanged(int current, int max)` | Callback thay đổi máu |

### Hint Stack API
```csharp
Manager.Instance.RegisterHint("text");    // push vào stack
Manager.Instance.UnregisterHint("text");  // pop khỏi stack
Manager.Instance.ShowDefaultHint();       // hiện default hint
Manager.Instance.SetDefaultHint(index);   // đổi default hint theo index
Manager.Instance.NextDefaultHint();       // chuyển sang hint tiếp theo
```

### Default Hints
- Khai báo list `defaultHints` trong Inspector
- `InitDefaultHint()` gọi trong `ContinueGame()` của manager con để reset về index 0
- Khi stack trigger trống → tự hiện default hint

---

## MapManager.cs

### Fields
| Field | Mô tả |
|-------|-------|
| `List<CinemachineCamera> machineCams` | Các camera cho từng vị trí |
| `float camTransitionDelay` | Delay blend cam trước khi hiện panel |
| `List<GameObject> storyPanels` | Danh sách story panel |
| `List<GameObject> soidlerPanels` | Danh sách soldier panel |
| `bool isWaveCleared` | Trạng thái wave |

### Cách dùng nhiều camera / panel
```
Trong Inspector:
  Machine Cams → [cam0, cam1, cam2, ...]
  Story Panels → [story0, story1, ...]
  Soidler Panels → [panel0, panel1, ...]

Từ trigger/code:
  Manager.Instance.ShowStory(1);                   // story thứ 2
  Manager.Instance.OpenSoilderPanel(0, 2);         // panel 0 + cam 2
```

### SoldierTrigger — gắn vào collider
```
Panel Index = 0   ← index trong soidlerPanels
Cam Index   = 1   ← index trong machineCams
```

---

## BaseManager.cs

| Field | Mô tả |
|-------|-------|
| `bool isWaveCleared` | Set true khi OnWaveCleared() |
| `Slider healthSlider` | Máu player |
| `TMP_Text clipSizeText` | Số đạn hiện tại |

**Lưu ý:** Phải có `Instance = this` trong `Awake()` để `BaseManager.Instance` hợp lệ.

---

## Thêm Manager mới

1. Tạo class kế thừa `Manager`
2. Thêm `public static NewManager Instance;`
3. Override `Awake()` → gọi `base.Awake()` và `Instance = this`
4. Override các virtual method cần thiết
5. Gắn script lên GameObject trong scene

---

## Callbacks từ gameplay về Manager

| Ai gọi | Method | Khi nào |
|--------|--------|---------|
| `WeaponAmmo` | `OnAmmoChanged` | Mỗi khi bắn / reload |
| `PlayerHealth` | `OnPlayerHealthChanged` | Mỗi khi máu thay đổi |
| `EnemySpawnZone` | `OnWaveCleared` | Khi tất cả enemy chết |
