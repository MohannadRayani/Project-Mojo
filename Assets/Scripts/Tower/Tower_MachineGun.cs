using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Tower_MachineGun : Tower
{
    private MachineGun_Visuals machineGunVisuals;

    [Header("Machine Gun Details")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float damage;
    [SerializeField] private float projectileSpeed;
    [Space]
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Transform[] gunPointSet;
    private int gunPointIndex;

    protected override void Awake()
    {
        base.Awake();
        machineGunVisuals = GetComponent<MachineGun_Visuals>();
        attackStrategy = new MachineGunAttackStrategy();
    }

    private sealed class MachineGunAttackStrategy : ITowerAttackStrategy
    {
        public void Execute(Tower tower)
        {
            Tower_MachineGun machineGun = (Tower_MachineGun)tower;
            machineGun.gunPoint = machineGun.gunPointSet[machineGun.gunPointIndex];
            Vector3 directionToEnemy = machineGun.DirectionToEnemyFrom(machineGun.gunPoint);

            if (Physics.Raycast(machineGun.gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity, machineGun.whatIsTargetable))
            {
                IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

                if (damagable == null)
                    return;

                GameObject newProjectile = machineGun.objectPool.Get(machineGun.projectilePrefab, machineGun.gunPoint.position, machineGun.gunPoint.rotation);
                newProjectile.GetComponent<Projectile_MachineGun>().SetupProjectile(hitInfo.point, damagable, machineGun.damage, machineGun.projectileSpeed, machineGun.objectPool);

                machineGun.machineGunVisuals.RecoilFx(machineGun.gunPoint);

                machineGun.lastTimeAttacked = Time.time;
                machineGun.gunPointIndex = (machineGun.gunPointIndex + 1) % machineGun.gunPointSet.Length;
            }
        }
    }

    protected override void RotateTowardsEnemy()
    {
        if (currentEnemy == null)
            return;

        Vector3 directionToEnemy = (currentEnemy.CenterPoint() - rotationOffset) - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        towerHead.rotation = Quaternion.Euler(rotation);
    }
}
