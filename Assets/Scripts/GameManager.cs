using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEditor;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    [SerializeField] EventSO eventSO;
    [SerializeField] GameObject startPage;
    [SerializeField] TMP_Text loadTMP;

    public bool gamePaused = false; 
    public bool isSave = false;
    public bool isLoad = false; //이어하기를 선택한 경우 true

    public List<Item> cardHandDataList = new List<Item>();
    public List<Item> itemBufferDataList = new List<Item>();
    public List<Item> dumpBufferDataList = new List<Item>();
    public List<Event> currentRequestBufferDataList = new List<Event>();

    string url_card = "https://docs.google.com/spreadsheets/d/17zZaaARc8KO4owFvPLsZ1uwmAqv_YEKf6vHkG515wkk/export?format=tsv&range=A2:E25";
    string url_req = "https://docs.google.com/spreadsheets/d/1taLaIVZSOraZ1wTDaW_WT64QXTfmYyuqhAURxTuhXMY/export?format=tsv&range=A2:K20";

    private void Start() {
        //시작화면 띄우기
        if(PlayerPrefs.HasKey("isSave") && PlayerPrefs.GetInt("isSave") == 1)
            loadTMP.GetComponentInChildren<TMP_Text>().color = new Color32(50, 50, 50, 255);
            
        startPage.transform.SetAsLastSibling();
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Default"));

        Time.timeScale = 0f;
        gamePaused = true;

        StartCoroutine(DownloadItemSO());
        StartCoroutine(DownloadEventSO());
    }

    //구글 스프레드 시트에서 카드 데이터 불러오기 
    IEnumerator DownloadItemSO()
    {
        UnityWebRequest www = UnityWebRequest.Get(url_card);
        yield return www.SendWebRequest();
        SetItemSO(www.downloadHandler.text);
    }

    //구글 스프레드 시트에서 이벤트 데이터 불러오기 
    IEnumerator DownloadEventSO()
    {
        UnityWebRequest www = UnityWebRequest.Get(url_req);
        yield return www.SendWebRequest();
        SetEventSO(www.downloadHandler.text);
        
    }

    public void GameStart()
    {
        startPage.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
        Camera.main.cullingMask = -1;
        AudioManager.Inst.PlaySound("click");

        PlayerPrefs.DeleteAll();
        isLoad = false;
        CardManager.Inst.SetupCardSystem();
        RequestManager.Inst.SetupRequesSystem();
        GaugeManager.Inst.SetupGaugeSystem();
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #else
            Application.Quit();
        #endif
    }

    public void GameLoad(){
        if(PlayerPrefs.HasKey("isSave") && PlayerPrefs.GetInt("isSave") == 1){
            AudioManager.Inst.PlaySound("click");
            isLoad = true;
            //cardHand 불러오기
            for(int i=0; i<PlayerPrefs.GetInt("cardHandCount"); i++){
                string cardName = PlayerPrefs.GetString("cardHand" + i.ToString());
                for (int j = 0; j < itemSO.items.Length; j++){
                    if (itemSO.items[j].inDeck && itemSO.items[j].name == cardName){
                        cardHandDataList.Add(itemSO.items[j]);
                        break;
                    }
                }
            }

            //card buffer 불러오기
            for(int i=0; i<PlayerPrefs.GetInt("itemBufferCount"); i++){
                string cardName = PlayerPrefs.GetString("itemBuffer" + i.ToString());
                for (int j = 0; j < itemSO.items.Length; j++){
                    if (itemSO.items[j].inDeck && itemSO.items[j].name == cardName){
                        itemBufferDataList.Add(itemSO.items[j]);
                        break;
                    }
                }
            }

            //dump buffer 불러오기
            for(int i=0; i<PlayerPrefs.GetInt("dumpBufferCount"); i++){
                string cardName = PlayerPrefs.GetString("dumpBuffer" + i.ToString());
                for (int j = 0; j < itemSO.items.Length; j++){
                    if (itemSO.items[j].inDeck && itemSO.items[j].name == cardName){
                        dumpBufferDataList.Add(itemSO.items[j]);
                        break;
                    }
                }
            }

            //current request buffer 불러오기
            for(int i=0; i<PlayerPrefs.GetInt("currentRequestBufferCount"); i++){
                string requestName = PlayerPrefs.GetString("currentRequestBuffer" + i.ToString());
                for(int j=0; j<eventSO.events.Length-3; j++){
                    if (eventSO.events[j].requestName == requestName){
                        currentRequestBufferDataList.Add(eventSO.events[j]);
                        break;
                    }
                }
            }

            //gauge 불러오기
            GaugeManager.Inst.faith = PlayerPrefs.GetInt("faith");
            GaugeManager.Inst.abundance = PlayerPrefs.GetInt("abundance");
            GaugeManager.Inst.refinement = PlayerPrefs.GetInt("refinement");
            GaugeManager.Inst.maxAbundance = PlayerPrefs.GetFloat("maxAbundance");
            GaugeManager.Inst.maxRefinement = PlayerPrefs.GetFloat("maxRefinement");

            startPage.SetActive(false);
            Time.timeScale = 1f;
            gamePaused = false;
            Camera.main.cullingMask = -1;

            CardManager.Inst.SetupCardSystem();
            RequestManager.Inst.SetupRequesSystem();
            GaugeManager.Inst.SetupGaugeSystem();
        }
        else 
            return;
    }

    //itemSO에 데이터 삽입
    void SetItemSO(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int colSize = row[0].Split('\t').Length;

        for(int i=0; i<rowSize; i++)
        {
            string[] col = row[i].Split('\t');
            Item targetItem = itemSO.items[i];

            for (int j=0; j<colSize; j++)
            {
                targetItem.type = col[0];
                targetItem.name = col[1];
                targetItem.content = col[2];
                targetItem.inDeck = (int.Parse(col[3]) == 1) ? true : false;
                targetItem.count = int.Parse(col[4]);
            }
        }
    }

    //eventSO에 데이터 삽입
    void SetEventSO(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int colSize = row[0].Split('\t').Length;

        for (int i = 0; i < rowSize; i++)
        {
            string[] col = row[i].Split('\t');
            Event targetEvent = eventSO.events[i];

            if (i >= rowSize - 3)
            {
                for (int j = 0; j < colSize; j++)
                {
                    targetEvent.requestName = col[0];
                    targetEvent.requestType = col[1];
                    targetEvent.requestContent = col[2];
                }
            }
            else
            {
                targetEvent.availableCard = new string[3];
                targetEvent.requestResultContent = new string[3];
                targetEvent.requestResultGauge = new int[2];

                for (int j = 0; j < colSize; j++)
                {
                    targetEvent.requestName = col[0];
                    targetEvent.requestType = col[1];
                    targetEvent.requestContent = col[2];

                    targetEvent.availableCard[0] = col[3];
                    targetEvent.availableCard[1] = col[4];
                    targetEvent.availableCard[2] = col[5];

                    targetEvent.requestResultContent[0] = col[6];
                    targetEvent.requestResultContent[1] = col[7];
                    targetEvent.requestResultContent[2] = col[8];

                    targetEvent.requestResultGauge[0] = int.Parse(col[9]);
                    targetEvent.requestResultGauge[1] = int.Parse(col[10]);
                    
                }
            }
        }
    }
}
