# SpaceWar — Project Overview

## Kiến trúc tổng quan

```
Manager (Singleton Base)
  ├── BaseManager       ← BaseScene
  ├── MapManager        ← Map/Arena scene
  ├── ControlRoomManager← ControlRoom scene
  └── MainSceneManager  ← MainScene (intro)

MovementBaseState (Abstract)
  ├── IdleState / WalkState / RunState / CrouchState / JumpState

ActionBaseState (Abstract)
  ├── DefaultState / ReloadState

AimBaseState (Abstract)
  ├── HipFireState / AimState
```

---

## Scenes

| Scene | Manager | Mô tả |
|-------|---------|-------|
| MainScene | MainSceneManager | Intro/title, hiển thị story |
| Map | MapManager | Arena chiến đấu, soldier panels, nhiều camera |
| BaseScene | BaseManager | Căn cứ ngầm, enemy waves, computer interaction |
| ControlRoom | ControlRoomManager | Phòng điều khiển |

---

## Design Patterns

| Pattern | Dùng ở |
|---------|--------|
| Singleton | GameManager, FadeManager, Manager (mọi Manager) |
| State Machine | MovementStateManager, ActionStateManager, AimStateManager |
| Event/Callback | OnAmmoChanged, OnPlayerHealthChanged, OnWaveCleared |
| Stack-based Priority | Hint System (RegisterHint / UnregisterHint) |
| NavMesh AI | AILocomotion, SkeletonAI, WolfbossAI |
| Poise / Stagger | WolfbossHealth (kiểu Dark Souls) |

---

## Flow chính

```
MainMenu
  → Button_Start → Loading_Manager → FadeManager.LoadScene()
  → MainScene → story → ContinueGame()
  → Map / BaseScene → gameplay loop
        ↓
    EnemySpawnZone.StartWave()
        ↓
    enemy chết → onDeath event → HandleEnemyDied()
        ↓ (tất cả chết)
    NotifyWaveCleared() → Manager.OnWaveCleared()
        ↓
    isWaveCleared = true → mở cửa / đổi hint / ...
```

---

## Files quan trọng

| File | Mục đích |
|------|----------|
| `Scripts/Manager/Manager.cs` | Base singleton, hint stack, callbacks |
| `Scripts/Enemy/EnemySpawnZone.cs` | Spawn + track wave |
| `Scripts/EnemyHealth.cs` | HP, ragdoll, death event |
| `Scripts/Weapon/WeaponManager.cs` | Bắn, fire rate, muzzle flash |
| `PhanCuaQuan/LopDuPhong/Scripts/AiLocomotion.cs` | AI patrol/chase/attack |
| `PhanCuaQuan/LopDuPhong/Scripts/BossWolf/WolfbossAI.cs` | Boss AI 2 phase |
