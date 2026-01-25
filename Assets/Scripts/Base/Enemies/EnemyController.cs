using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;//적 데이터
    public NavMeshAgent agent;//적 추적용 컴퍼넌트, NavMesh Agent
    public Transform player;//씬 속 플레이어

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();//NavMesh Agent 컴퍼넌트 가져오기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");//플레이어 오브젝트 찾기
        if (playerObj != null) player = playerObj.transform;//플레이어를 찾으면 위에 있던 변수에 대입
    }
}