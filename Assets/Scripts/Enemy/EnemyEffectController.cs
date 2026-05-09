public interface IEnemyEffectController
{
    void SlowEnemy(float slowMultiplier, float duration);
    void DisableHide(float duration);
    void HideEnemy(float duration);
}

public class EnemyEffectController : IEnemyEffectController
{
    protected readonly Enemy enemy;

    public EnemyEffectController(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void SlowEnemy(float slowMultiplier, float duration)
    {
        enemy.ApplySlowEnemy(slowMultiplier, duration);
    }

    public virtual void DisableHide(float duration)
    {
        enemy.ApplyDisableHide(duration);
    }

    public virtual void HideEnemy(float duration)
    {
        enemy.ApplyHideEnemy(duration);
    }
}