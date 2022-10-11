using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Inst { get; private set; }
    void Awake() => Inst = this;

    public int faith; //신앙심(50시작, 0~125)
    public int abundance;//풍요(50시작, 0~100)
    public int refinement;//교양(0시작, 0-4 / 0-8 / 0-12 )

    [SerializeField] Image faithGaugeMask;
    [SerializeField] Image abundanceGaugeMask;
    [SerializeField] Image refinementGaugeMask;
    [SerializeField] TMP_Text faithGaugeTMP;
    [SerializeField] TMP_Text abundanceGaugeTMP;
    [SerializeField] TMP_Text refinementGaugeTMP;
    [SerializeField] GameObject gameOverPage;
    [SerializeField] GameObject gameClearPage;
    
    public float maxFaith = 125f;
    public float maxAbundance = 100f;
    public float maxRefinement = 4f;

    float faithGaugeOrgSize;
    float abundanceGaugeOrgSize;
    float refinementGaugeOrgSize;

    bool isDelay = false;
    bool isDarkAge = false;

    private void Update()
    {
        if(faith <= 0 || abundance <= 0){
            DOTween.KillAll();
            PlayerPrefs.SetInt("isSave", 0);
            GameManager.Inst.isSave = false;
            GameManager.Inst.isLoad = false;

            gameOverPage.SetActive(true);
            Time.timeScale = 0f;
            GameManager.Inst.gamePaused = true;
            gameOverPage.transform.SetAsLastSibling();
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Default"));
        }

        if(refinement == 12){
            DOTween.KillAll();
            PlayerPrefs.SetInt("isSave", 0);
            GameManager.Inst.isSave = false;
            GameManager.Inst.isLoad = false;

            gameClearPage.SetActive(true);
            Time.timeScale = 0f;
            GameManager.Inst.gamePaused = true;
            gameClearPage.transform.SetAsLastSibling();
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Default"));
        }

        //신앙심 게이지가 최대치에 도달하면 암흑시대 페널티를 부여하고(풍요 게이지가 지속적으로 감소), 화면을 흑백으로 전환
        if(isDarkAge && !isDelay && abundance > 0){
            isDelay = true;
            CameraEffect.Inst.intensity = 50;
            StartCoroutine(DecreaseAbundance());
        }
    }

    public void SetupGaugeSystem(){
        //게이지 높이 데이터 받아오기
        faithGaugeOrgSize = faithGaugeMask.rectTransform.rect.height;
        abundanceGaugeOrgSize = abundanceGaugeMask.rectTransform.rect.height;
        refinementGaugeOrgSize = refinementGaugeMask.rectTransform.rect.height;

        //게이지 이미지 초기화
        faithGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, faithGaugeOrgSize * (faith/maxFaith)); //시작 신앙 게이지: 50/125
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance)); //시작 풍요 게이지: 50/100
        refinementGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, refinementGaugeOrgSize * (refinement/maxRefinement)); //시작 교양 게이지: 0/4

        //게이지 초기값 표시
        faithGaugeTMP.text = (faith + "/" + maxFaith).ToString();
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
        refinementGaugeTMP.text = (refinement + "/" + maxRefinement).ToString();
    }

    //암흑시대 페널티 - 풍요 게이지 지속적으로 감소 구현
    IEnumerator DecreaseAbundance()
    { 
        yield return new WaitForSeconds(1.0f);
        abundance-=2;
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance));
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
        isDelay = false;
    }

    //신앙심 게이지 변화 반영
    public void UpdateFaithGauge(int value)
    {
        faith += value;

        //신앙심 게이지가 최대치에 도달하면 암흑시대로 전환됨
        if(faith >= maxFaith)
        {
            faith = (int)maxFaith;
            isDarkAge = true;
        }

        //신앙심 게이지가 최대치에서 100으로 감소하면 암흑시대 해제
        if(faith <= 100)
        {
            isDarkAge = false;
            CameraEffect.Inst.intensity = 0;
        }

        if(faith <= 0)
            faith = 0;

        faithGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, faithGaugeOrgSize * (faith/maxFaith));
        faithGaugeTMP.text = (faith + "/" + maxFaith).ToString();
    }

    //풍요 게이지 변화 반영
    public void UpdateAbundanceGauge(int value)
    {
        abundance += value;

        if(abundance > maxAbundance)
            abundance = (int)maxAbundance;

        if(abundance <=0)
            abundance = 0;

        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance));
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
    }

    //교양 게이지 변화 반영
    public void UpdateRefinementGauge(int value, bool isMain = false)
    {   
        //교양 게이지가 최대치에 도달했을 때, 교양을 0으로 하고 최대치를 4/8/12로 변환
        if (isMain) {
            refinement = 0;
            maxRefinement += 4;
        }
        else 
            refinement += value;

        if (refinement < 0) 
            refinement = 0;

        refinementGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, refinementGaugeOrgSize * (refinement/maxRefinement));
        refinementGaugeTMP.text = (refinement + "/" + maxRefinement).ToString();
    }

    //재해 카드가 남아있는 경우, 풍요 게이지의 최대치를 감소시킴  
    public void UpdateMaxAbundance()
    {
        maxAbundance -= 5;
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance));
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
    }
}
