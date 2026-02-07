using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    // [변경 1] 인스펙터에서 여러 종류의 프리팹과 초기 개수를 설정할 수 있도록 클래스 정의
    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int initialSize = 20;
    }

    public List<PoolItem> poolItems; // 인스펙터에서 할당

    // [변경 2 & 요청사항] 단일 리스트 대신, 프리팹 이름을 키(Key)로 사용하는 딕셔너리로 변경
    // 생성된 오브젝트들은 Instantiate 시점에 이 스크립트가 붙은 오브젝트의 자식이 됩니다.
    private Dictionary<string, List<GameObject>> _poolDictionary = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (var item in poolItems)
        {
            if (item.prefab == null) continue;

            string key = item.prefab.name;
            // 딕셔너리에 해당 키가 없으면 리스트 생성
            if (!_poolDictionary.ContainsKey(key))
            {
                _poolDictionary[key] = new List<GameObject>();
            }

            for (int i = 0; i < item.initialSize; i++)
            {
                GameObject obj = CreateNewObject(item.prefab);
                obj.SetActive(false);
                _poolDictionary[key].Add(obj);
            }
        }
    }

    // [변경 3] 가져올 때 어떤 프리팹을 원하는지 인자로 받아야 함
    public GameObject GetBullet(GameObject prefabToGet)
    {
        string key = prefabToGet.name;

        // 1. 해당 프리팹 풀이 딕셔너리에 없으면 새로 생성 (안전장치)
        if (!_poolDictionary.ContainsKey(key))
        {
            _poolDictionary[key] = new List<GameObject>();
        }

        // 2. 비활성화된 오브젝트 찾기
        foreach (GameObject obj in _poolDictionary[key])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // 3. 풀이 모자라면 확장
        GameObject newObj = CreateNewObject(prefabToGet);
        newObj.SetActive(true);
        _poolDictionary[key].Add(newObj);
        return newObj;
    }

    // [요청사항 구현 핵심] 오브젝트 생성 및 자식 설정 도우미 함수
    private GameObject CreateNewObject(GameObject prefab)
    {
        // Instantiate(원본, 부모Transform) -> 이렇게 하면 생성되자마자 자식으로 들어갑니다.
        GameObject obj = Instantiate(prefab, transform);
        
        // (선택사항) 생성된 오브젝트 이름 뒤에 (Clone)이 붙는데, 깔끔하게 관리하고 싶다면 아래 주석 해제
        // obj.name = prefab.name; 
        
        return obj;
    }
}