using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Crossbow : Tower
{
    private Crossbow_Visuals visuals;

    [Header("Crossbow details")]
    [SerializeField] private int damage;

    protected override void Awake()
    {
        base.Awake();
        visuals = GetComponent<Crossbow_Visuals>();
        attackStrategy = new CrossbowAttackStrategy();
    }

    private sealed class CrossbowAttackStrategy : ITowerAttackStrategy
    {
        public void Execute(Tower tower)
        {
            Tower_Crossbow crossbow = (Tower_Crossbow)tower;
            crossbow.lastTimeAttacked = Time.time;

            Vector3 directionToEnemy = crossbow.DirectionToEnemyFrom(crossbow.gunPoint);

            if (Physics.Raycast(crossbow.gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity, crossbow.whatIsTargetable))
            {
                crossbow.towerHead.forward = directionToEnemy;

                IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(crossbow.damage);
                }

                crossbow.visuals.CreateOnHitFx(hitInfo.point);
                crossbow.visuals.PlayAttackVFX(crossbow.gunPoint.position, hitInfo.point);
                crossbow.visuals.PlayReloaxVFX(crossbow.attackCooldown);
                AudioManager.instance?.PlaySFX(crossbow.attackSfx, true);
            }
        }
    }
}
