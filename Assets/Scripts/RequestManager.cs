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
    [SerializeField] Transform requestSpawnPoint;
    //[SerializeField] Transform[] requestIconPoints;
    [SerializeField] Button[] requestIcon; // 0 = main / 1 = normal / 2 = disaster / 3 = special
    [SerializeField] Sprite[] requestIconImgList;
    [SerializeField] int turn;

    List<Event> requestBuffer;
    List<Event> mainRequestBuffer;
    List<Event> currentRequestBuffer;

    GameObject requestObject;
    int currentIndex;
    bool isAccept;
    bool isExcept;

    public bool isRequestActive = false; //요청 창 활성화 여부

    private void Start()
    {
        SetupRequestBuffer();
    }

    private void Update()
    {
        if (!isRequestActive)
            Invoke(nameof(UpdateRequestIcon), 0.5f);
    }

    //요청 리스트 기본 설정하기
    void SetupRequestBuffer()
    {
        requestBuffer = new List<Event>();
        mainRequestBuffer = new List<Event>();
        currentRequestBuffer = new List<Event>();

        for(int i=0; i<eventSO.events.Length; i++)
        {
            if (i >= eventSO.events.Length - 3)
            {
                Event mainRequest = eventSO.events[i];
                mainRequestBuffer.Add(mainRequest);
            }
            else
            {
                Event request = eventSO.events[i];
                requestBuffer.Add(request);
            } 
        }

        ShuffleRequestBuffer();

        for (int i = 0; i < 5; i++)
        {
            currentRequestBuffer.Add(requestBuffer[i]);
        }
    }


    //버튼이 시간차를 두고 동적으로 생성됨. 생성되는 요청들의 타입은 일반 또는 재해
    void UpdateRequestIcon()
    {
        int requestTypeIndex = 1;

        for (int i = 0; i < 5; i++)
        {
            if (!isRequestActive && !requestIcon[i].IsActive())
            {
                requestIcon[i].gameObject.SetActive(true);

                if (currentRequestBuffer[i].requestType == "normal")
                    requestTypeIndex = 1;
                else if (currentRequestBuffer[i].requestType == "disaster")
                    requestTypeIndex = 2;

                requestIcon[i].GetComponent<Image>().sprite = requestIconImgList[requestTypeIndex];
                requestIcon[i].onClick.AddListener(() => 
                { 
                    if (!isRequestActive) 
                        ShowRequest(i); 
                });

                break;
            }

        }
    }

    void ShuffleRequestBuffer()
    {
        for (int i = 0; i < requestBuffer.Count; i++)
        {
            int index = Random.Range(i, requestBuffer.Count);
            Event tmp = requestBuffer[i];
            requestBuffer[i] = requestBuffer[index];
            requestBuffer[index] = tmp;
        }
    }

    //요청 보여주기
    void ShowRequest(int requestIdx)
    {
        isRequestActive = true;
        isAccept = false;
        isExcept = false;
        currentIndex = requestIdx;

        requestIcon[currentIndex].gameObject.SetActive(false);

        if (requestObject == null)
        {
            requestObject = Instantiate(requestPopUp, requestSpawnPoint.position, Quaternion.identity);
            var request = requestObject.GetComponent<Request>();

            Event selectedRequest = currentRequestBuffer[currentIndex];
            request.Setup(currentRequestBuffer[currentIndex]);

            request.requestInnerBtn.onClick.AddListener(() => ShowResult());
        }
    }

    void ShowMainRequest(int mainRequestIdx)
    {
        requestObject = Instantiate(requestPopUp, requestSpawnPoint.position, Quaternion.identity);
        var request = requestObject.GetComponent<Request>();

        request.Setup(mainRequestBuffer[mainRequestIdx]);
        request.requestInnerBtn.onClick.AddListener(() => {
            DestroyImmediate(requestObject);
            isRequestActive = false;
        });

        mainRequestBuffer.RemoveAt(mainRequestIdx);
        GaugeManager.Inst.UpdateRefinementGauge(0, true);
        turn++;
    }

    //요청 처리하기
    public void HandleRequest(Card selectedCard)
    {
        var currentRequest = currentRequestBuffer[currentIndex];
       
        if (currentRequest.requestResultContent[2] != "" && selectedCard.item.type == "hazard")
        {
            isAccept = true;
            isExcept = true;
        }
        else
        {
            if (selectedCard.item.name == "고요" && currentRequest.requestType == "disaster") isAccept = true;
            if (selectedCard.item.name == "전능" && currentRequest.requestType == "normal") isAccept = true;
            
            foreach (var card in currentRequest.availableCard)
            {
                if (selectedCard.item.name == card)
                {
                    isAccept = true;
                    break;
                }
            }
            
        }

        if (!isAccept && selectedCard.item.type == "hazard") CardManager.Inst.AddDisasterCard(selectedCard);

        ShowResult();
    }

    void HandleGauge()
    {
        int faithGauge = currentRequestBuffer[currentIndex].requestResultGauge[0];
        int abundanceGauge = currentRequestBuffer[currentIndex].requestResultGauge[1];

        if (!isAccept)
        {
            faithGauge = -faithGauge;
            abundanceGauge = -abundanceGauge;
        }

        if (isExcept) faithGauge += 5;

        GaugeManager.Inst.UpdateFaithGauge(faithGauge);
        GaugeManager.Inst.UpdateAbundanceGauge(abundanceGauge);
        GaugeManager.Inst.UpdateRefinementGauge((isAccept) ? 1 : 0);
    }

    void ShowResult()
    {
        HandleGauge();
        int refinement = GaugeManager.Inst.getRefinementGauge();

        DestroyImmediate(requestObject);

        requestObject = Instantiate(requestPopUp, requestSpawnPoint.position, Quaternion.identity);
        var request = requestObject.GetComponent<Request>();

        request.Setup(currentRequestBuffer[currentIndex], true, isAccept, isExcept);

        request.requestInnerBtn.onClick.AddListener(() => { 
            DestroyImmediate(requestObject);
            isRequestActive = false;

            if ((turn == 0 && refinement == 3) ||
            (turn == 1 && refinement == 5) ||
            (turn == 2 && refinement == 10))
                ShowMainRequest(turn);
            
        });

        Event currentRequest = currentRequestBuffer[currentIndex];
        currentRequestBuffer.RemoveAt(currentIndex);

        ShuffleRequestBuffer();
        currentRequestBuffer.Insert(currentIndex, requestBuffer[0]);
    }
}
