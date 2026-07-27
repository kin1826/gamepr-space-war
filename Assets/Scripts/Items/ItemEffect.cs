using UnityEngine;

// Lớp cơ sở cho hành vi có thể gắn vào ItemData.
// Trả về true khi effect đã áp dụng thành công; khi đó inventory mới trừ item.
public abstract class ItemEffect : ScriptableObject
{
    public abstract bool TryApply(GameObject user);
}
