using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Hammer : Tower
{
    private Hammer_Visuals hammerVisuals;

    [Header("Hammer details")]

    [Range(0,1)]
    [SerializeField] private float slowMultiplier = .4f;
    [SerializeField] private float slowDuration;

    protected override void Awake()
    {
        base.Awake();
        hammerVisuals = GetComponent<Hammer_Visuals>();
        attackStrategy = new HammerAttackStrategy();
    }
    protected override void FixedUpdate()
    {
        if (towerActive == false)
            return;

        if (CanAttack())
            Attack();
    }

    private sealed class HammerAttackStrategy : ITowerAttackStrategy
    {
        public void Execute(Tower tower)
        {
            Tower_Hammer hammer = (Tower_Hammer)tower;
            hammer.lastTimeAttacked = Time.time;
            hammer.hammerVisuals.PlayAttackAnimation();

            foreach (var enemy in hammer.ValidEnemyTargets())
            {
                enemy.SlowEnemy(hammer.slowMultiplier, hammer.slowDuration);
            }
        }
    }

    private List<Enemy> ValidEnemyTargets()
    {
        List<Enemy> targets = new List<Enemy>();
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsTargetable);
        
        foreach (Collider enemy in enemiesAround)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();

            if (newEnemy != null) 
                targets.Add(newEnemy);
        }

        return targets;
    }

    protected override bool CanAttack()
    {
        return Time.time > lastTimeAttacked + attackCooldown && AtLeastOneEnemyAround();
    }

}
