using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    string url = "https://docs.google.com/spreadsheets/d/17zZaaARc8KO4owFvPLsZ1uwmAqv_YEKf6vHkG515wkk/export?format=tsv&range=A2:E22";

    private void Start()
    {
        StartCoroutine(DownloadItemSO());    
    }

    IEnumerator DownloadItemSO()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        SetItemSO(www.downloadHandler.text);
    }

    void SetItemSO(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int colSize = row[0].Split('\t').Length;

        for(int i=0; i<rowSize; i++)
        {
            string[] col = row[i].Split('\t');

            for(int j=0; j<colSize; j++)
            {
                Item targetItem = itemSO.items[i];

                targetItem.type = col[0];
                targetItem.name = col[1];
                targetItem.content = col[2];
                targetItem.inDeck = (int.Parse(col[3]) == 1) ? true : false;
                targetItem.count = int.Parse(col[4]);
            }
        }
    }
}
