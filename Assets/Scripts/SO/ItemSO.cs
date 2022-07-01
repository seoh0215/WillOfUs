using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    //이름, 종류, 내용, 카드틀, 카드이미지, 개수, 덱 포함 여부

    public string name;
    public string type;
    public string content;
    public Sprite template;
    public Sprite cardImage;
    public int count;
    public bool inDeck;
}

[CreateAssetMenu(fileName ="ItemSO", menuName ="ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}
