using UnityEngine;

// Khuôn dữ liệu cho 1 loại vật phẩm. Mỗi vật phẩm cụ thể là 1 file .asset
// tạo ra từ class này (chuột phải trong Project > Create > Item > Item Data),
// không cần viết thêm code cho từng item.
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Nhận diện")]
    public string id;
    public string itemName;

    [Header("Hiển thị HUD")]
    public Sprite icon;
    public int maxStackSize;
    [TextArea] public string description;
}
