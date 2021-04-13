using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance {
        get {
            if (_instance == null)
                Debug.LogError("PoolManager instance is NULL!");

            return _instance;
        }
    }

    public enum PoolType {
        Laser,
        Enemy
    }

    [Header("Laser Pool Settings")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Transform _laserParent;
    [SerializeField]
    private int _initialLaserPoolSize;

    [Header("Enemy Pool Settings")]
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private Transform _enemyParent;
    [SerializeField]
    private int _initialEnemyPoolSize;

    private List<GameObject> _laserPool;
    private List<GameObject> _enemyPool;

    // When adding a new pool type, remember to add new pool type to enum PoolType, IdentifyPool method
    // and AddToPool method. Create the pool in the Start method.

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        _laserPool = GeneratePool(_laserPrefab, _laserParent, _initialLaserPoolSize);
        _enemyPool = GeneratePool(_enemyPrefab, _enemyParent, _initialEnemyPoolSize);
    }

    List<GameObject> GeneratePool(GameObject prefab, Transform parent, int count) {

        List<GameObject> generatedPool = new List<GameObject>();
        for (int i = 0; i < count; i++) {
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
            newObj.SetActive(false);
            generatedPool.Add(newObj);
        }

        return generatedPool;
    }

    public GameObject RequestPoolMember(Vector3 position, PoolType pool) {

        List<GameObject> requestedPool = IdentifyPool(pool);

        if (requestedPool == null) {
            Debug.LogError("Pool " + pool + " not found!");
            return null;
        }
        else {
            for (int i = 0; i < requestedPool.Count; i++) {
                if (!requestedPool[i].activeInHierarchy) {
                    requestedPool[i].SetActive(true);
                    requestedPool[i].transform.position = position;
                    return requestedPool[i];
                }
            }
        }

        // If no remaining inactive pool members
        return AddToPool(position, pool, requestedPool);
    }

    List<GameObject> IdentifyPool(PoolType pool) {

        switch (pool) {
            case PoolType.Laser:
                return _laserPool;
            case PoolType.Enemy:
                return _enemyPool;
            default:
                Debug.LogWarning("Placeholder for null return from PoolManager.IdentifyPool()");
                return null;
        }
    }

    GameObject AddToPool(Vector3 position, PoolType pool, List<GameObject> requestedPool) {

        GameObject newMember;

        switch (pool) {

            case PoolType.Laser:
                newMember = GenerateNewPoolMember(_laserPrefab, _laserParent, position);
                requestedPool.Add(newMember);
                _laserPool = requestedPool;
                return newMember;

            case PoolType.Enemy:
                newMember = GenerateNewPoolMember(_enemyPrefab, _enemyParent, position);
                requestedPool.Add(newMember);
                _enemyPool = requestedPool;
                return newMember;

            default:
                Debug.LogWarning("Placeholder for null return from PoolManager.AddToPool()");
                return null;
        }
    }

    GameObject GenerateNewPoolMember(GameObject prefab, Transform parent, Vector3 position) {

        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        obj.transform.position = position;
        return obj;
    }

    public void ReturnPoolMember(GameObject obj) {
        obj.transform.position = Vector3.zero;
        obj.SetActive(false);
    }

    struct PoolInfo {

        // Potential future implementation

        GameObject prefab;
        Transform parent;
        PoolType poolType;
        int initalPoolSize;

        public PoolInfo(GameObject prefab, Transform parent, PoolType poolType, int initalPoolSize) {
            this.prefab = prefab;
            this.parent = parent;
            this.poolType = poolType;
            this.initalPoolSize = initalPoolSize;
        }
    }
}
