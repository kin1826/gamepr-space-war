# SpaceWar — Last Stand of the Exodus

> *Tàu mẹ đã bị chiếm. Chỉ còn một người có thể giành lại nó.*

---

## Giới thiệu

**SpaceWar** là game hành động bắn súng góc nhìn thứ nhất (FPS) với bối cảnh khoa học viễn tưởng. Người chơi nhập vai Alex — phi công chiến đấu tinh nhuệ — trong một cuộc chiến sinh tử trên con tàu mẹ khổng lồ đã bị kẻ thù chiếm đóng.

---

## Cốt truyện

### Thế giới

Trong một góc xa xôi của vũ trụ, hành tinh quê hương của Alex từng là nơi yên bình… cho đến khi bị một thế lực cổ xưa mang tên **Chúa tể bóng tối** xâm lược. Chúng không chỉ tàn sát mà còn biến cư dân thành nô lệ, khai thác tài nguyên và biến cả hành tinh thành một pháo đài sống.

Một số ít người sống sót đã kịp trốn thoát, tập hợp trên một **tàu mẹ khổng lồ** — hy vọng cuối cùng để tìm kiếm một hành tinh mới.

---

### Biến cố

Trong một chuyến trinh sát độc lập, Alex nhận được tín hiệu khẩn cấp:

> *"Alex… chúng đã tìm ra chúng tôi. Hệ thống phòng thủ đang sụp đổ. Chúng đã chiếm được khu lõi năng lượng, hệ thống vũ khí và các trạm điều khiển chính… Chúng tôi cần cậu… ngay lập tức."*
> — **Đội trưởng Nana-chan**

Alex lập tức quay về — nhưng khi đến nơi, tàu mẹ đã trở thành **một chiến hạm bị chiếm đóng**, hệ thống phòng thủ bị hack và quay ngược lại chống chính chủ nhân của nó.

---

## Các giai đoạn gameplay

### Giai đoạn 1 — Công Phá Từ Bên Ngoài
*(Scene: MainScene)*

Alex điều khiển phi thuyền chiến đấu, một mình đối đầu với:
- Các tàu chiến tự động bị kiểm soát
- Hệ thống pháo phòng thủ bị chiếm quyền
- Drone và chiến hạm của Chúa tể bóng tối

**Mục tiêu:** Phá vỡ lớp phòng thủ ngoài và mở đường xâm nhập vào tàu mẹ.

---

### Giai đoạn 2 — Chiến Đấu Bên Trong
*(Scene: BaseScene → Map)*

Sau khi đột nhập thành công, Alex chuyển sang chiến đấu bộ binh bên trong tàu. Phần lớn đồng đội đã bị tiêu diệt hoặc bắt giữ — một số ít còn ẩn nấp trong các khu kỹ thuật.

**Nhiệm vụ:**
| Mục tiêu | Ý nghĩa |
|---|---|
| **Lõi năng lượng (Power Core)** | Ngăn tàu bị phá hủy |
| **Hệ thống điều hướng (Navigation)** | Tránh tàu lao vào vùng chết |
| **Kho vũ khí (Armory Control)** | Mở khóa trang bị mạnh hơn |
| **Hệ thống AI trung tâm** | Ngắt kết nối kẻ thù khỏi tàu |

---

### Giai đoạn 3 — Trận Chiến Cuối
*(Scene: ControlRoomScene)*

Điểm đến cuối cùng là **Phòng Điều Khiển Trung Tâm (Command Bridge)**. Tại đây, biết không thể chống lại, Chúa tể bóng tối đã kích hoạt tự hủy và để lại **Sói Chiến — Boss cấp cao** được nuôi dưỡng cho mục đích chiến tranh — để cản trở Alex đến cùng.

---

## Kết cục

**Chiến thắng:**
- Alex giành lại quyền kiểm soát tàu mẹ
- Giải cứu những người còn sống sót
- Mở ra hy vọng mới cho nhân loại

**Thất bại:**
> Tàu mẹ — hy vọng cuối cùng — sẽ trở thành vũ khí của kẻ thù.

---

## Tính năng

- FPS kết hợp điều khiển phi thuyền
- Hệ thống wave enemy với boss
- Hệ thống nhân vật: level, XP, vàng, kim cương
- Nhạc nền động thay đổi theo tình huống (thường / boss)
- Hiệu ứng UI: fade, zoom, slider reward
- Hỗ trợ pause, respawn, death camera

---

## Công nghệ

- **Engine:** Unity (URP)
- **Ngôn ngữ:** C#
- **AI:** NavMesh Agent
- **Camera:** Cinemachine
- **Input:** Unity Input System
- **Lưu dữ liệu:** JSON (PlayerData)
