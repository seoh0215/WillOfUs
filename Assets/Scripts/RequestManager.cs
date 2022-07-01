using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Inst { get; private set; }
    void Awake() => Inst = this;

    //필요한 목록: eventSO, 요청 창, 요청이 나타날 위치, 요청 아이콘 이미지, 요청 아이콘 위치
    [SerializeField] EventSO eventSO;
    [SerializeField] GameObject requestPopUp;
    [SerializeField] Button callRequestBtn;
    [SerializeField] Transform requestSpawnPoint;
    [SerializeField] Transform[] requestIconPoints;

    List<Event> requestList;
    GameObject requestObject;
    Event currentRequest;
    Button requestInnerBtn;
    bool isAccept;

    public bool isRequestActive = false; //요청 창 활성화 여부

    private void Start()
    {
        SetupRequestList();
        callRequestBtn.onClick.AddListener(ShowRequest);
    }

    //요청 리스트 기본 설정하기
    void SetupRequestList()
    {
        requestList = new List<Event>();

        for(int i=0; i<eventSO.events.Length; i++)
        {
            Event request = eventSO.events[i];
            requestList.Add(request);
        }
    }

    //요청 보여주기
    void ShowRequest()
    {
        DestroyImmediate(callRequestBtn.gameObject);

        requestObject = Instantiate(requestPopUp, requestSpawnPoint.position, Quaternion.identity);
        requestInnerBtn = requestPopUp.GetComponentInChildren<Button>();
        var request = requestObject.GetComponent<Request>();
        int index = Random.Range(0, requestList.Count);

        isRequestActive = true;
        isAccept = false;
        currentRequest = requestList[0];
        request.Setup(currentRequest);
        requestInnerBtn.onClick.AddListener(() => ShowResult());
    }

    //요청 처리하기
    public void HandleRequest(Card selectedCard)
    {
        isRequestActive = false;

        foreach (var card in currentRequest.availableCard)
        {
            if (selectedCard.item.name == card)
            {
                isAccept = true;
                break;
            }
        }

        ShowResult();
    }

    void ShowResult()
    {
        DestroyImmediate(requestObject);

        requestObject = Instantiate(requestPopUp, requestSpawnPoint.position, Quaternion.identity);
        var request = requestObject.GetComponent<Request>();

        request.Setup(currentRequest, true, isAccept);
    }
}
