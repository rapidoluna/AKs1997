using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 50;

    private Queue<GameObject> _poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab, transform);
            obj.SetActive(false);
            _poolQueue.Enqueue(obj);
        }
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        if (_poolQueue.Count > 0)
        {
            GameObject obj = _poolQueue.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(bulletPrefab, position, rotation, transform);
            return obj;
        }
    }

    public void ReturnBullet(GameObject obj)
    {
        obj.SetActive(false);
        _poolQueue.Enqueue(obj);
    }
}