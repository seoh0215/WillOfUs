using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer cardTemplate;
    [SerializeField] SpriteRenderer cardImage;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text contentTMP;

    public Item item;
    public PRS originPRS;

    public void Setup(Item item)
    {
        this.item = item;
        cardTemplate.sprite = this.item.template;
        cardImage.sprite = this.item.cardImage;
        nameTMP.text = this.item.name.ToString();
        contentTMP.text = this.item.content.ToString();
    }

    //카드 이동
    public void MoveTransform(PRS prs, float dotweenTime = 0)
    {
        transform.DOMove(prs.position, dotweenTime);
        transform.DORotateQuaternion(prs.rotation, dotweenTime);
        transform.DOScale(prs.scale, dotweenTime);
    }

    private void OnMouseOver()
    {
        CardManager.Inst.CardMouseOver(this);
    }

    private void OnMouseExit()
    {
        CardManager.Inst.CardMouseExit(this);
    }

    private void OnMouseDown()
    {
        if(RequestManager.Inst.isRequestActive)
            CardManager.Inst.CardMouseDrag();

        if (CardManager.Inst.cardSelectPopUp.activeSelf)
            CardManager.Inst.CardMouseClick(this);
    }

    private void OnMouseUp()
    {
        if (RequestManager.Inst.isRequestActive)
            CardManager.Inst.CardMouseUp();
    }
}
