# Player & Weapon System

---

## Player

### PlayerControl.cs — nhân vật đi bộ

Dùng khi player rời tàu (`GameManager.EnterPlayer()`).

| Field | Default | Mô tả |
|-------|---------|-------|
| `walkSpeed` | 2.5 | Tốc độ đi |
| `runSpeed` | 6 | Tốc độ chạy |
| `gravity` | -19.62 | Gravity |
| `jumpHeight` | 1.5 | Chiều cao nhảy |
| `mouseSensitivity` | 0.05 | Độ nhạy chuột |

### PlayerController.cs — điều khiển tàu

Dùng khi trong tàu (`GameManager.EnterShip()`).

| Field | Default | Mô tả |
|-------|---------|-------|
| `baseSpeed` | 2 | Tốc độ tối thiểu |
| `maxSpeed` | 100 | Tốc độ tối đa |
| `acceleration` | 20 | Gia tốc |
| `rollAngle` | 25° | Góc nghiêng |

Camera tự zoom out theo tốc độ tàu (`CameraSpeedOffset`).

### PlayerHealth.cs (PhanCuaQuan)

| Field | Default | Mô tả |
|-------|---------|-------|
| `maxHealth` | 200 | Máu tối đa |
| `invincibleDuration` | 0.5s | I-frame sau khi bị hit |

**Events:**
```csharp
playerHealth.OnHealthChanged += (current, max) => { };
playerHealth.OnDeath += () => { };
```

**API:**
```csharp
playerHealth.TakeDamage(20);   // nhận damage (có i-frame check)
playerHealth.Heal(50);          // hồi máu
```

Manager nhận callback tự động qua `Manager.Instance.OnPlayerHealthChanged(current, max)`.

---

## State Machines

### Movement States

```
Idle ←→ Walk ←→ Run
  ↕         ↕
Crouch    Jump
```

| State | Vào khi | Ra khi |
|-------|---------|--------|
| Idle | không input | WASD pressed |
| Walk | WASD | release WASD / Shift / C / Space |
| Run | Shift + WASD | release Shift |
| Crouch | C | C lại |
| Jump | Space | chạm đất |

Speeds tùy chỉnh trong `MovementStateManager` Inspector.

### Action States (bắn/reload)

```
Default ←→ Reload
```

- `Default`: IK weight = 1, check R để reload
- `Reload`: disable IK, trigger animation

Animation Events callback về `ActionStateManager`:
- `WeaponReloaded()` — reload xong
- `MagOut()` / `MagIn()` / `ReleaseSlide()` — sound effects

### Aim States (hip / ADS)

```
HipFire ←→ AimState (RMB)
```

- `Alt` để đổi vai (shoulder swap)
- ADS giảm FOV → zoom nhẹ
- Raycast center screen → `aimPos` để tính hướng bắn

---

## Weapon System

### WeaponManager.cs

| Field | Mô tả |
|-------|-------|
| `fireRate` | Giây giữa 2 phát |
| `semiAuto` | true = 1 click 1 phát, false = giữ bắn liên tục |
| `bullet` | Bullet prefab |
| `bulletsPerShot` | Số đạn mỗi phát (shotgun = nhiều) |
| `bulletVelocity` | Vận tốc đạn |
| `damage` | Damage mỗi viên |
| `enemyKickbackForce` | Lực đẩy lùi khi giết enemy |

### WeaponAmmo.cs

| Field | Mô tả |
|-------|-------|
| `clipSize` | Dung tích băng đạn |
| `extraAmmo` | Đạn dự phòng |
| `currentAmmo` | Đạn hiện tại trong băng |

**API:**
```csharp
ammo.UseAmmo();   // trừ 1 đạn + notify Manager
ammo.Reload();    // reload + notify Manager
```

Reload logic thông minh:
- Đủ đạn dự phòng → fill đầy clip
- Thiếu → dùng hết extra
- Partial reload → cộng dồn

### WeaponBloom.cs — Spread

Spread tăng khi: di chuyển > đứng yên, hip > ADS.

Multipliers chỉnh trong Inspector.

### Bullet.cs

- Va chạm enemy → `TakeDamge(weapon.damage)`
- Phát kết liễu → thêm kickback force theo hướng bay
- Tự destroy sau `timeToDestroy` giây

---

## Camera System

### CameraSwitch.cs
Toggle FP/TP camera với input `SwitchCamera`.
Lock khi đang ở chế độ tàu.

### CameraSpeedOffset.cs
Tự động zoom out theo tốc độ tàu:
- `minDistance` → tốc độ thấp
- `maxDistance` → tốc độ max

### AimStateManager.cs
- Manage camera follow + FOV transition
- Raycast từ center screen → `aimPos` (điểm ngắm thực tế)
- `barrelPos.LookAt(aimPos)` để bắn chính xác
