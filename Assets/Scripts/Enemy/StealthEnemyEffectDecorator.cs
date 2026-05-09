using System.Collections;
using UnityEngine;

public class StealthEnemyEffectDecorator : EnemyEffectController
{
    private readonly Enemy_Stealth stealthEnemy;

    public StealthEnemyEffectDecorator(IEnemyEffectController wrapped, Enemy_Stealth stealthEnemy) : base((Enemy)stealthEnemy)
    {
        this.stealthEnemy = stealthEnemy;
        inner = wrapped;
    }

    private IEnemyEffectController inner;

    public override void SlowEnemy(float slowMultiplier, float duration)
    {
        inner.SlowEnemy(slowMultiplier, duration);
    }

    public override void DisableHide(float duration)
    {
        stealthEnemy.StartCoroutine(DisableSmokeCo(duration));
        inner.DisableHide(duration);
    }

    public override void HideEnemy(float duration)
    {
        inner.HideEnemy(duration);
    }

    private IEnumerator DisableSmokeCo(float duration)
    {
        stealthEnemy.EnableSmoke(false);
        stealthEnemy.SetCanHideEnemies(false);

        yield return new WaitForSeconds(duration);

        stealthEnemy.EnableSmoke(true);
        stealthEnemy.SetCanHideEnemies(true);
    }
}