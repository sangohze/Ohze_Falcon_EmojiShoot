using UnityEngine;

public abstract class WeaponGameWinHandlerBase : MonoBehaviour, IWeaponGameWinHandler
{
    public abstract void OnEnemyTargetHit(CharacterController enemy);
    public abstract void SetTickPreviewByEnemy(EmojiType emoji);
}
