using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Request : MonoBehaviour
{
    [SerializeField] TMP_Text requestContent;
    [SerializeField] TMP_Text requestResultContent;
    [SerializeField] Button requestInnerBtn;

    public Event request;

    public void Setup(Event request, bool isResult = false, bool isAccept = true)
    {
        this.request = request;
        requestContent.text = this.request.requestContent.ToString(); //요청 내용
        requestInnerBtn.GetComponentInChildren<TMP_Text>().text = "넘기기";

        if (isResult)
        {
            if (!isAccept) requestResultContent.text = this.request.requestResultContent[0].ToString(); //요청 실패시 내용
            else requestResultContent.text = this.request.requestResultContent[1].ToString(); //요청 성공시 내용
            requestInnerBtn.GetComponentInChildren<TMP_Text>().text = "확인";
        }
    }
}
