public interface IWeaponGameWinHandler
{
    void OnEnemyTargetHit(CharacterController enemy);
    void SetTickPreviewByEnemy(EmojiType emoji);
}