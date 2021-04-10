using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance {
        get {
            if (_instance == null)
                Debug.LogError("PoolManager is NULL!");

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

    private List<GameObject> _laserPool = new List<GameObject>();
    private List<GameObject> _enemyPool = new List<GameObject>();

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
            GameObject newObj = Instantiate(prefab);
            newObj.transform.parent = parent;
            newObj.SetActive(false);
            generatedPool.Add(newObj);
        }

        return generatedPool;
    }

    GameObject GenerateNewPoolMember(GameObject prefab, Transform parent, Vector3 position) {

        GameObject obj = Instantiate(prefab);
        obj.transform.parent = parent;
        obj.transform.position = position;
        return obj;
    }

    public GameObject RequestPoolMember(Vector3 position, PoolType pool) {

        List<GameObject> requestedPool = new List<GameObject>();
        switch (pool) {
            case PoolType.Laser:
                requestedPool = _laserPool;
                break;
            case PoolType.Enemy:
                requestedPool = _enemyPool;
                break;
        }

        for (int i = 0; i < requestedPool.Count; i++) {
            if (!requestedPool[i].activeInHierarchy) {
                requestedPool[i].SetActive(true);
                requestedPool[i].transform.position = position;
                return requestedPool[i];
            }
        }

        // If no remaining inactive pool members
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
                Debug.LogError("Pools Empty");
                return null;
        }
    }

    public void ReturnPoolMember(GameObject obj) {
        obj.SetActive(false);
    }
}
