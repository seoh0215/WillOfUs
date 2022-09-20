using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject gaugeUI;
    [SerializeField] List<Card> cardHand;
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] Transform cardDumpPoint;
    [SerializeField] Transform cardSelectPoint;
    [SerializeField] Transform leftCardPos;
    [SerializeField] Transform rightCardPos;
    [SerializeField] Button drawAgainBtn;

    public GameObject cardSelectPopUp;
    public int cardHandCount;

    List<Item> itemBuffer;
    List<Item> dumpBuffer;
    Card selectedCard;
    bool isCardSelect = false;
    
    private void Start()
    {
        SetupCardBuffer();
        for (int i = 0; i < cardHandCount; i++)
            AddCard();
        drawAgainBtn.onClick.AddListener(drawAgain);
    }

    private void Update()
    {
        if (isCardSelect)
            DragCard();

    }

    //카드 뽑기
    public Item PopCard()
    {
        if (itemBuffer.Count == 0)
        {
            SetupCardBuffer();
            dumpBuffer.Clear();
        }
        
        Item card = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return card;
    }

    public Item PopFromDumpBuffer()
    {
        if(dumpBuffer.Count > 1)
        {
            for (int i = 0; i < dumpBuffer.Count; i++)
            {
                int index = Random.Range(i, dumpBuffer.Count);
                Item tmp = dumpBuffer[i];
                dumpBuffer[i] = dumpBuffer[index];
                dumpBuffer[index] = tmp;
            }
        }
        Item card = dumpBuffer[0];
        dumpBuffer.RemoveAt(0);
        return card;
    }

    void SetupCardBuffer()
    {
        itemBuffer = new List<Item>();
        dumpBuffer = new List<Item>();

        //기본 덱 만들기
        for (int i = 0; i < itemSO.items.Length; i++) 
        {
            Item card = itemSO.items[i];
            for (int j = 0; j < card.count; j++) 
            {
                if(card.inDeck)
                    itemBuffer.Add(card);
            }
        }

        //덱 섞기
        for (int i = 0; i < itemBuffer.Count; i++) 
        {
            int index = Random.Range(i, itemBuffer.Count);
            Item tmp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[index];
            itemBuffer[index] = tmp;
        }
    }

    //카드 간 order in layer 설정
    void SetOriginOrder()
    {
        for (int i = 0; i < cardHand.Count; i++)
        {
            var card = cardHand[i];
            card.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    //카드 뽑은 후 처리
    void AddCard(bool isFromDumpBuffer = false)
    {
        Transform spawnPoint = isFromDumpBuffer ? cardDumpPoint : cardSpawnPoint;
        var cardObj = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity);
        var card = cardObj.GetComponent<Card>();

        if (!isFromDumpBuffer)
            card.Setup(PopCard());
        else if (isFromDumpBuffer && dumpBuffer.Count > 0)
            card.Setup(PopFromDumpBuffer());
        else
            return;

        cardHand.Add(card);
        SetOriginOrder();
        CardAlignment();
    }

    //카드 뽑을 때 이동
    void CardAlignment()
    {
        List<PRS> originPRSList = new List<PRS>();
        originPRSList = RoundAlignment(leftCardPos, rightCardPos, cardHand.Count, 0.5f);

        for (int i = 0; i < cardHand.Count; i++)
        {
            var card = cardHand[i];
            card.originPRS = originPRSList[i];
            card.MoveTransform(card.originPRS, 0.5f);
        }
    }

    //패 둥글게 펼치기
    List<PRS> RoundAlignment(Transform leftCardPos, Transform rightCardPos, int objCount, float height)
    {
        float[] objLerps = new float[objCount];
        List<PRS> res = new List<PRS>(objCount);

        switch (objCount) {
            case 1:
                objLerps = new float[] { 0.5f };
                break;
            case 2:
                objLerps = new float[] { 0.3f, 0.7f };
                break;
            case 3:
                objLerps = new float[] { 0.2f, 0.5f, 0.8f };
                break;
            default:
                float interval = 1f / objCount;
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftCardPos.position, rightCardPos.position, objLerps[i]);
            var targetRot = Quaternion.identity;

            float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
            curve = (height >= 0) ? curve : -curve;
            targetPos.y += curve;
            targetRot = Quaternion.Lerp(leftCardPos.rotation, rightCardPos.rotation, objLerps[i]);
            
            res.Add(new PRS(targetPos, targetRot, Vector3.one));
        }

        return res;
    }

    //카드에 마우스 커서 올리면 확대
    void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.position.x, -5.0f, 0.0f);
            card.MoveTransform(new PRS(enlargePos, Quaternion.identity, Vector3.one * 1.2f));
        }
        else
            card.MoveTransform(card.originPRS);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    //카드 드래그시 마우스 커서에 카드가 따라옴
    void DragCard()
    {
        selectedCard.MoveTransform(new PRS(Utils.MousePos, Quaternion.identity, Vector3.one));
    }

    //카드 버리기
    void DumpCard(Card card)
    {
        dumpBuffer.Add(card.item);
        cardHand.Remove(card);

        card.transform.DOKill();
        isCardSelect = false;
        DestroyImmediate(card.gameObject);
        selectedCard = null;
        CardAlignment();
    }

    //버릴 카드 선택
    void SelectDumpCard(Card card)
    {
        selectedCard = card;
        isCardSelect = true;
        DumpCard(selectedCard);
        cardSelectPopUp.SetActive(false);
        gaugeUI.SetActive(true);
    }

    //카드 사용
    void UseCard()
    {
        string currentCardType = selectedCard.item.type;
        string currentCardName = selectedCard.item.name;

        if (currentCardType == "ability")
        {
            switch (currentCardName) //ability(권능)카드 효과
            {
                case "재생":
                    AddCard();
                    break;

                case "윤회":
                    if (dumpBuffer.Count > 0)
                        AddCard(true);
                    break;

                case "희생":
                    cardSelectPopUp.SetActive(true);
                    gaugeUI.SetActive(false);
                    AddCard();
                    AddCard();
                    break;

                case "초월":
                    int size = cardHand.Count;
                    for (int i = 0; i < size; i++)
                    {
                        dumpBuffer.Add(cardHand[i].item);
                        if (cardHand[i] == selectedCard) continue;
                        DestroyImmediate(cardHand[i].gameObject);
                    }
                    cardHand.Clear();

                    for (int i = 0; i < 5; i++)
                        AddCard();
                    break;

                case "고요":
                    RequestManager.Inst.HandleRequest(selectedCard);
                    break;

                case "전능":
                    RequestManager.Inst.HandleRequest(selectedCard);
                    break;

                default:
                    break;
            }
        }
        else
        {
            RequestManager.Inst.HandleRequest(selectedCard);
        }

        DumpCard(selectedCard);
    }

    public void AddDisasterCard(Card selectedCard)
    {
        var cardObj = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
        var card = cardObj.GetComponent<Card>();

        for (int i = 0; i < itemSO.items.Length; i++)
        {
            if (itemSO.items[i].type == "disaster" && itemSO.items[i].name == selectedCard.item.name)
                card.Setup(itemSO.items[i]);
        }

        cardHand.Add(card);
        SetOriginOrder();
        CardAlignment();
    }

    void drawAgain()
    {
        if (!RequestManager.Inst.isRequestActive)
        {
            foreach (var card in cardHand)
            {
                if(card.item.type == "disaster")
                    GaugeManager.Inst.UpdateMaxAbundance();
                dumpBuffer.Add(card.item);
                card.transform.DOKill();
                DestroyImmediate(card.gameObject);
            }
            cardHand.Clear();
            for (int i = 0; i < cardHandCount; i++)
            {
                AddCard();
            }

            GaugeManager.Inst.UpdateRefinementGauge(-1);
        }
    }

    public void CardMouseOver(Card card)
    {
        if(!isCardSelect)
            selectedCard = card;

        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDrag() //마우스 드래그시 카드 선택 = true
    {
        isCardSelect = true;
    }

    public void CardMouseUp() //마우스 드래그를 놓으면 카드 사용
    {
        if (!Input.GetMouseButton(1))
        {
            UseCard();
        }
        else
        {
            if (isCardSelect)
            { 
                isCardSelect = false;
            }
        }
    }

    public void CardMouseClick(Card card)
    {
        SelectDumpCard(card);
    }
}
