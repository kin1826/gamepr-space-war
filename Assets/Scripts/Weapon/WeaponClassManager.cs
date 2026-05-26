using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponClassManager : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    public Transform recoiFollowPos;
    ActionStateManager actions;

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        if(actions == null) actions = GetComponent<ActionStateManager>();
        leftHandIK.data.target = weapon.leftHandTarget;
        leftHandIK.data.hint = weapon.leftHandHint;
        actions.SetWeapon(weapon);
    }
}
