using UnityEngine;

[CreateAssetMenu(fileName = "OperatorData", menuName = "AKs97/Operator Data")]
public class OperatorData : ScriptableObject
{
    [Header("Extract")]
    public string[] successComments = {
        "무한한 폭력 속에서 할당량을 채워 오셨군요.",
        "오다레스는 기쁩니다.",
        "당신에게 순수 코드의 축복을 내리겠습니다.",
        "고기라도 쏠까요."
    };

    [Header("Death")]
    public string[] kiaComments = {
        "에이스의 몸을 가지고서 루키의 모습을 보인다는 것. 아아, 이 얼마나 아름다운 모순인가.",
        "당신도 그들의 일부가 됐습니다.",
        "순수 코드로 변하다니...죽어도 쓸모가 있는게, 얼마나 좋아요.",
        "제물이 되어라!"
    };
}