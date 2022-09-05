using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeManager : MonoBehaviour
{
    public static GaugeManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] int faith; //신앙심(50시작, 0~125)
    [SerializeField] int abundance;//풍요(50시작, 0~100)
    [SerializeField] int refinement;//교양(0시작, 0-3 / 0-5 / 0-10 )

    public int getFaithGauge() { return faith; }
    public int getAbundanceGauge() { return abundance; }
    public int getRefinementGauge() { return refinement; }

    public void UpdateFaithGauge(int value)
    {
        faith += value;
        print("신앙심: " + faith);
    }

    public void UpdateAbundanceGauge(int value)
    {
        abundance += value;
    }

    public void UpdateRefinementGauge(int value, bool isMain = false)
    {
        if (isMain) refinement = 0;
        else refinement += value;

        if (refinement < 0) refinement = 0;
        print("교양: " + refinement);
    }

    void ReflectFaithGauge()
    {
        
    }

    void ReflectAbundanceGauge()
    {
        //70이상인 경우 카드 추가 이벤트
    }
}
