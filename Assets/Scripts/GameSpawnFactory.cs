using UnityEngine;

public class GameSpawnFactory : SpawnFactory
{
    private ObjectPoolManager objectPool;

    protected override void Awake()
    {
        base.Awake();
        objectPool = ObjectPoolManager.instance;
    }

    public override GameObject CreateEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        return objectPool.Get(enemyPrefab, position, rotation);
    }

    public override GameObject CreateTower(GameObject towerPrefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(towerPrefab, position, rotation);
    }
}