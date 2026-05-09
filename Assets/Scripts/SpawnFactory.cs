using UnityEngine;

public abstract class SpawnFactory : MonoBehaviour
{
    public static SpawnFactory instance;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public abstract GameObject CreateEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation);
    public abstract GameObject CreateTower(GameObject towerPrefab, Vector3 position, Quaternion rotation);
}