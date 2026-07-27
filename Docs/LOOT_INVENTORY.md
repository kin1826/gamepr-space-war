# Loot rương và balo

Tài liệu này mô tả luồng loot item hiện tại trong `BaseScene`.

## Thành phần chính

| Script | Trách nhiệm |
|---|---|
| `ItemData` | Dữ liệu cố định của một loại item: `id`, tên, icon, mô tả và `maxStackSize`. |
| `CrateInteract` | Trigger rương, mở rương và nhận từng `LootEntry` theo lần bấm `F`. |
| `LootPanelUI` | Hiển thị danh sách item còn lại trong rương. |
| `InventorySystem` | Lưu các stack trong balo trong thời gian chơi. |
| `InventoryController` | Bật/tắt panel balo bằng `Tab` và dựng UI item trong Content. |

## Cấu hình ItemData

`maxStackSize` là số lượng tối đa trong **một stack**, không phải số lượng item đang có hoặc số loot rương cho.

Ví dụ:

| Item | Max Stack Size | Ý nghĩa |
|---|---:|---|
| Đạn | 30 | Một stack đạn chứa tối đa 30 viên. |
| Túi máu | 1 | Mỗi túi máu chiếm một stack. |
| Coin | 9999 | Một stack coin chứa tối đa 9999 coin. |

Nếu `maxStackSize` là `0` hoặc âm, code an toàn và coi giá trị là `1`.

## Luồng rương

1. Player đi vào `BoxCollider` có bật `Is Trigger` của rương.
2. Khi rương chưa mở, hint hiển thị `Press F to open`.
3. Bấm `F` lần đầu chỉ mở nắp rương, phát âm thanh và hiện panel loot.
4. Các lần bấm `F` sau nhận lần lượt từng `LootEntry` theo thứ tự trong `lootEntries`.
5. Mỗi entry có `Item` và `Quantity`; player nhận chính xác `Quantity`, sau đó inventory tự chia stack theo `maxStackSize`.
6. Panel loot cập nhật để chỉ hiển thị item còn lại. Khi hết item, hint là `Chest is empty`.

Rương chỉ mở một lần. Chức năng bỏ item khỏi balo chưa được triển khai.

## Stack balo

`InventorySystem` lưu `List<InventorySlot>`. Mỗi slot gồm:

```csharp
ItemData item;
int quantity;
```

Khi nhận item, hệ thống ưu tiên cộng vào stack cùng loại đang chưa đầy, sau đó tạo stack mới. Balo không giới hạn số slot.

Ví dụ: nhận 60 đạn với `maxStackSize = 30` sẽ tạo hai stack: `30` và `30`.

## Cấu hình Unity Inspector

### Balo

1. Chọn một Canvas hoặc UI Manager luôn active.
2. Gắn cả `InventorySystem` và `InventoryController` vào object đó.
3. Trong `InventoryController`, gán:
   - `Inventory Panel`: panel balo, mặc định inactive.
   - `Items Container`: `Content` trong Scroll View (hoặc object có `Grid Layout Group`).
   - `Inventory Item Prefab`: `Assets/Prefab/Loot_item.prefab`.
4. Bấm `Tab` để bật/tắt balo. Mở balo sẽ hiện chuột và chặn input gameplay; đóng balo sẽ khoá/ẩn chuột và mở lại input gameplay.

`Loot_item.prefab` được dùng chung cho panel loot rương và balo. Prefab có `Image` cho icon; nếu có `TMP_Text`, UI balo sẽ hiển thị quantity của stack.

### Rương

1. Gắn `CrateInteract` lên object có `BoxCollider` với `Is Trigger` bật.
2. Gán `Lid`, `Open Sfx`, `Loot Panel` và danh sách `Loot Entries` theo thứ tự muốn loot.
3. Với mỗi entry, đặt `Item` và `Quantity` mong muốn, ví dụ Bullet × 15 hoặc Coin × 100.
4. Player phải có tag `Player` để trigger hoạt động.

## Đạn lấy từ balo

Để một súng dùng đạn trong balo, gán asset đạn vào field `Inventory Ammo Item` của `WeaponAmmo`.

- Player bắt đầu với một băng đầy: `clipSize / 0` (ví dụ `30 / 0`).
- Mỗi phát bắn chỉ giảm đạn trong băng đang lắp, không giảm đạn dự trữ trong balo.
- Loot đạn tăng số dự trữ và HUD cập nhật ngay (ví dụ `30 / 30`).
- Reload chuyển đúng số đạn cần thiết từ balo vào băng. Ví dụ `15 / 60` reload thành `30 / 45`.
- Khi inventory mất đạn do reload, stack về `0` sẽ bị xoá.
- Panel balo đang mở sẽ tự refresh khi loot hoặc reload.
- Không có cơ chế vứt item; quantity trong inventory là nguồn dữ liệu duy nhất.

## Dùng item và quick slot 4

`ItemData` có thể chứa nhiều `ItemEffect`. Inventory chỉ trừ một item khi ít nhất một effect trả về thành công.

Hiện đã có `HealEffect`:

1. Trong Project, tạo asset bằng **Create > Item Effects > Heal**.
2. Đặt `Heal Amount`, ví dụ `40`.
3. Mở asset ItemData của bình máu và thêm HealEffect vào list `Effects`.
4. Player đã dùng `PlayerHealth` có sẵn ở `Assets/PhanCuaQuan/LopDuPhong/Scripts/PlayerHealth.cs`; chỉ cần gắn `QuickSlotController` lên Player.
5. Kéo ItemData của bình máu vào `Slot 4 Item` trong QuickSlotController.

Bấm `4` sẽ dùng bình máu. Bình chỉ bị trừ khi player chưa đầy máu và hồi máu thành công. Nếu thêm các effect mới trong tương lai, tạo class kế thừa `ItemEffect` và gắn asset effect đó vào ItemData.

## Giới hạn hiện tại

- Inventory chỉ tồn tại trong phiên chơi; chưa có save/load.
- Mỗi `LootEntry` trong rương tương ứng một lần bấm `F`.
- Chưa có thao tác chọn, dùng, di chuyển hoặc bỏ item khỏi balo.
