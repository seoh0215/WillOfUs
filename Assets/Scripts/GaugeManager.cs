using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] int faith; //신앙심(50시작, 0~125)
    [SerializeField] int abundance;//풍요(50시작, 0~100)
    [SerializeField] int refinement;//교양(0시작, 0-4 / 0-8 / 0-12 )

    [SerializeField] Image faithGaugeMask;
    [SerializeField] Image abundanceGaugeMask;
    [SerializeField] Image refinementGaugeMask;

    [SerializeField] TMP_Text faithGaugeTMP;
    [SerializeField] TMP_Text abundanceGaugeTMP;
    [SerializeField] TMP_Text refinementGaugeTMP;

    float faithGaugeOrgSize;
    float abundanceGaugeOrgSize;
    float refinementGaugeOrgSize;

    float maxFaith = 125f;
    float maxAbundance = 100f;
    float maxRefinement = 4f;

    bool isDelay = false;
    bool isDarkAge = false;

    public int getFaithGauge() { return faith; }
    public int getAbundanceGauge() { return abundance; }
    public int getRefinementGauge() { return refinement; }

    private void Start()
    {
        faithGaugeOrgSize = faithGaugeMask.rectTransform.rect.height;
        abundanceGaugeOrgSize = abundanceGaugeMask.rectTransform.rect.height;
        refinementGaugeOrgSize = refinementGaugeMask.rectTransform.rect.height;

        faithGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, faithGaugeOrgSize * (faith/maxFaith)); //시작 신앙 게이지: 50/125
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance)); //시작 풍요 게이지: 50/100
        refinementGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, refinementGaugeOrgSize * (refinement/maxRefinement)); //시작 교양 게이지: 0/4

        faithGaugeTMP.text = (faith + "/" + maxFaith).ToString();
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
        refinementGaugeTMP.text = (refinement + "/" + maxRefinement).ToString();
    }  

    private void Update()
    {
        if(isDarkAge && !isDelay && abundance > 0){
            isDelay = true;
            CameraEffect.Inst.intensity = 50;
            StartCoroutine(DecreaseAbundance());
        }
    }

    IEnumerator DecreaseAbundance()
    { 
        yield return new WaitForSeconds(1.0f);
        abundance-=2;
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance));
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
        isDelay = false;
    }

    public void UpdateFaithGauge(int value)
    {
        faith += value;

        if(faith >= maxFaith)
        {
            faith = (int)maxFaith;
            isDarkAge = true;
        }

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

    public void UpdateRefinementGauge(int value, bool isMain = false)
    {
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

    public void UpdateMaxAbundance()
    {
        maxAbundance -= 5;
        abundanceGaugeMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, abundanceGaugeOrgSize * (abundance/maxAbundance));
        abundanceGaugeTMP.text = (abundance + "/" + maxAbundance).ToString();
    }
}
