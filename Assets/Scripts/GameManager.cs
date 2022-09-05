using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    [SerializeField] EventSO eventSO;
    string url_card = "https://docs.google.com/spreadsheets/d/17zZaaARc8KO4owFvPLsZ1uwmAqv_YEKf6vHkG515wkk/export?format=tsv&range=A2:E25";
    string url_req = "https://docs.google.com/spreadsheets/d/1taLaIVZSOraZ1wTDaW_WT64QXTfmYyuqhAURxTuhXMY/export?format=tsv&range=B2:K20";

    private void Start()
    {
        StartCoroutine(DownloadItemSO());
        StartCoroutine(DownloadEventSO());
    }

    IEnumerator DownloadItemSO()
    {
        UnityWebRequest www = UnityWebRequest.Get(url_card);
        yield return www.SendWebRequest();
        SetItemSO(www.downloadHandler.text);
    }

    IEnumerator DownloadEventSO()
    {
        UnityWebRequest www = UnityWebRequest.Get(url_req);
        yield return www.SendWebRequest();
        SetEventSO(www.downloadHandler.text);
    }

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
                    targetEvent.requestType = col[0];
                    targetEvent.requestContent = col[1];
                }
            }
            else
            {
                targetEvent.availableCard = new string[3];
                targetEvent.requestResultContent = new string[3];
                targetEvent.requestResultGauge = new int[2];

                for (int j = 0; j < colSize; j++)
                {
                    targetEvent.requestType = col[0];
                    targetEvent.requestContent = col[1];

                    targetEvent.availableCard[0] = col[2];
                    targetEvent.availableCard[1] = col[3];
                    targetEvent.availableCard[2] = col[4];

                    targetEvent.requestResultContent[0] = col[5];
                    targetEvent.requestResultContent[1] = col[6];
                    targetEvent.requestResultContent[2] = col[7];

                    targetEvent.requestResultGauge[0] = int.Parse(col[8]);
                    targetEvent.requestResultGauge[1] = int.Parse(col[9]);
                }
            }
        }
    }
}
