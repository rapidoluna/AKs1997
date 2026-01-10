using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 50;

    private List<GameObject> _pool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            _pool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        foreach (GameObject obj in _pool)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        GameObject newObj = Instantiate(bulletPrefab);
        newObj.SetActive(false);
        _pool.Add(newObj);
        return newObj;
    }
}