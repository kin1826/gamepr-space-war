# Quick Reference — SpaceWar

## Thêm tính năng mới nhanh

### Thêm hint tại 1 điểm trong scene
1. Tạo Empty GameObject, thêm Collider → Is Trigger = true
2. Gắn `HintTrigger`
3. Điền `Hint Text`

### Thêm enemy wave mới
1. Tạo Empty GameObject, gắn `EnemySpawnZone`
2. Kéo enemy prefab vào `Enemy Prefab`
3. Chỉnh `Max Enemies`, `Zone Size`
4. Gizmo màu cam hiện vùng spawn trong Editor

### Thêm soldier panel / camera mới (MapManager)
1. Tạo panel UI, gắn `UIPanelFader`
2. Kéo vào `Soidler Panels` list trong MapManager Inspector
3. Tạo CinemachineCamera, kéo vào `Machine Cams` list
4. Gắn `SoldierTrigger` lên collider, set `Panel Index` + `Cam Index`

### Thêm story mới (MapManager)
1. Tạo panel UI, gắn `UIPanelFader` + `StoryDialogue`
2. Kéo vào `Story Panels` list trong MapManager
3. Gọi `Manager.Instance.ShowStory(index)` từ trigger/code

### Thêm default hint
1. Trong Inspector của Manager con, thêm vào `Default Hints` list
2. Gọi `Manager.Instance.SetDefaultHint(index)` khi muốn đổi

### Thêm Manager cho scene mới
1. Copy ControlRoomManager.cs làm template
2. Đổi tên class + static Instance
3. Override các method cần thiết
4. Gắn lên GameObject trong scene

---

## Các typo cần biết

| Code | Thực tế |
|------|---------|
| `TakeDamge()` | Là `TakeDamage` nhưng typo trong codebase |
| `soidler_Panel` | Là `soldier_Panel` |
| `soilder` | Là `soldier` |

---

## Inspector Checklist khi setup scene mới

- [ ] Có Manager phù hợp trong scene
- [ ] FadeManager có trong scene (hoặc load từ scene trước)
- [ ] EventSystem trong scene (để UI click hoạt động)
- [ ] Player GameObject có tag `"Player"`
- [ ] EnemyHealth prefab có `RagdollManager`
- [ ] Slider healthSlider: Min=0, Max tự set qua code
- [ ] CinemachineCamera machine mặc định **disabled** (enabled = false)

---

## Thứ tự gọi khi game start

```
1. FadeManager.Start() → isFading = true
2. Manager.Start() → chờ fade xong → ShowStory(0)
3. Player nhấn F → ContinueGame() → gameplay bắt đầu
4. EnemySpawnZone.Start() → StartWave()
5. ...combat...
6. Wave clear → Manager.OnWaveCleared() → isWaveCleared = true
7. ComputerInteraction → cho phép mở cửa
```

---

## Debug Tips

| Vấn đề | Kiểm tra |
|--------|---------|
| Hint không hiện | `Manager.Instance` có null không? Player có tag "Player"? |
| Wave không clear | Enemy có `EnemyHealth`? Spawn qua `EnemySpawnZone` chứ không phải drag vào scene |
| Camera không đổi | `CinemachineCamera` có disabled ban đầu không? Priority có cao hơn cam khác? |
| AI không di chuyển | Bake NavMesh chưa? `playerTransform` null? |
| Đạn không trừ | Gọi `ammo.UseAmmo()` chứ không phải `ammo.currentAmmo--` |
| Máu không cập nhật UI | `BaseManager.Instance` có được set trong Awake không? |
