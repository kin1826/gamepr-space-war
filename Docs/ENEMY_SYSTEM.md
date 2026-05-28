# Enemy System

---

## EnemyHealth.cs

Gắn lên root của enemy prefab.

| Field | Mô tả |
|-------|-------|
| `maxHealth` | Máu tối đa (default 200) |
| `destroyDelay` | Giây chờ trước khi Destroy (default 3f, để ragdoll chạy) |
| `hpCanvas` | Canvas hiện HP bar (tự quay về phía camera) |
| `hpSlider` | Slider hiển thị máu |

### Events
```csharp
enemyHealth.onDeath += () => { /* callback khi chết */ };
```

### HP Bar
- Tự hiện khi bị hit
- Tự ẩn khi chết
- Chỉ hiện khi enemy nằm trong góc nhìn camera (`dot > 0.5f`)
- Canvas luôn xoay mặt về phía camera (LateUpdate)

### Flow khi chết
```
TakeDamge(damage)
  → health <= 0 → EnemyDeath()
    → isDead = true
    → ẩn hpCanvas
    → RagdollManager.TriggerRagdoll()
    → onDeath?.Invoke()          ← EnemySpawnZone nhận
    → Destroy(gameObject, destroyDelay)
```

**Lưu ý:** Tên method là `TakeDamge` (typo, không phải `TakeDamage`).

---

## EnemySpawnZone.cs

Gắn lên Empty GameObject làm vùng spawn.

| Field | Mô tả |
|-------|-------|
| `enemyPrefab` | Prefab enemy |
| `maxEnemies` | Số lượng 1 wave |
| `spawnInterval` | Giây giữa mỗi lần spawn |
| `zoneSize` | Vector3 kích thước vùng (Gizmo màu cam) |

Gizmo màu da cam hiện trong Editor để thấy vùng spawn.

**API:**
```csharp
spawnZone.StartWave();    // bắt đầu
spawnZone.CancelWave();   // hủy + destroy hết enemy còn sống
```

---

## AILocomotion.cs (Enemy cơ bản)

Gắn lên enemy có NavMeshAgent + Animator.

### States
```
Patrol → Chase → Attack
  ↑_____________________|  (player chạy quá xa)
```

| State | Điều kiện chuyển |
|-------|-----------------|
| Patrol → Chase | Player vào `detectionRange` + trong `fieldOfView` |
| Chase → Attack | Khoảng cách ≤ `attackRange` |
| Attack → Chase | Player chạy ra ngoài `attackRange + 0.5f` |
| Chase → Patrol | Player > `detectionRange * 1.5f` |

### Inspector Fields
| Field | Default | Mô tả |
|-------|---------|-------|
| `detectionRange` | 20 | Tầm phát hiện |
| `attackRange` | 2 | Tầm tấn công |
| `fieldOfView` | 120° | Góc nhìn |
| `patrolSpeed` | 1.5 | Tốc độ patrol |
| `chaseSpeed` | 3 | Tốc độ đuổi |
| `attackCooldown` | 1.5s | Thời gian giữa 2 đòn |
| `attackHitDelay` | 0.4s | Delay từ trigger anim đến gây damage |
| `attackDamage` | 10 | Damage mỗi đòn |
| `patrolPoints` | optional | Waypoints patrol cố định |
| `randomPatrolRadius` | 8 | Bán kính random patrol nếu không có waypoints |

### Tự tìm Player
Không cần kéo player vào. Tự tìm qua tag `"Player"` khi vào Update lần đầu.
Nếu scene dùng tag khác, gán thủ công vào `playerTransform`.

### Animator Parameters
| Parameter | Type | Khi nào |
|-----------|------|---------|
| `Speed` (default) | Float | Velocity.magnitude liên tục |
| `Attack` (default) | Trigger | Mỗi lần attack |

---

## SkeletonAI.cs (Enemy nâng cao)

Require: NavMeshAgent, Animator, SkeletonHealth.

### States
`Waiting → Chase → Attack / Scream → Dead`

### Tấn công weighted random
```
Melee weights: Slash1=40%, Slash2=30%, Stab=20%, Scream=10%
```

- `Scream`: buff speed, tăng `moveSpeed` tạm thời
- Có thể ném dao ở tầm xa (`throwRange`)

---

## WolfbossAI.cs (Boss)

Require: NavMeshAgent, Animator, WolfbossHealth.

### States
`Idle → Chase → Attack / Stagger → Dead`

### Phase 2
Khi HP ≤ 30% → `EnterPhase2()` → tăng tốc, đổi tỉ lệ tấn công:
```
Phase 1: Atk1=60%, Atk2=25%, Atk3=15%
Phase 2: Atk1=45%, Atk2=45%, Atk3=10%
```

### Poise System (Dark Souls style)
- `maxPoise = 100f`: Chịu đựng trước khi stagger
- `poiseRegenRate = 20f/s`: Tốc độ hồi phục sau `poiseRegenDelay = 2s`
- Khi poise về 0: `OnPoiseBreak()` → State.Stagger

```csharp
// Gây damage + poise damage
wolfHealth.TakeDamage(damage: 20, poiseDamage: 30f);
```
