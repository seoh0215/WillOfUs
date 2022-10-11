using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Event
{
    public string requestName; //요청의 이름
    public string requestType; //요청의 종류
    public string requestContent; //요청의 내용
    public string[] availableCard; //해결 가능한 카드
    public string[] requestResultContent; //요청 결과창 내용
    public int[] requestResultGauge;
    //해결시 변동되는 게이지. 순서대로 신앙심, 풍요, 교양
    //원소만큼 성공하면 증가하지만 실패하면 감소함
}

[CreateAssetMenu(fileName = "EvnetSO", menuName = "ScriptableObject/EventSO")]
public class EventSO : ScriptableObject
{
    public Event[] events;
}
