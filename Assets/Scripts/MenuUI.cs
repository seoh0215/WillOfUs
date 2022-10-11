using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] GameObject menuPopup;
    [SerializeField] GameObject savePopup;
    [SerializeField] Button settingBtn;

    private void Update() {
        settingBtn.onClick.AddListener(() => {
            menuPopup.SetActive(true);
            Time.timeScale = 0f;
            GameManager.Inst.gamePaused = true;
            menuPopup.transform.SetAsLastSibling();
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Default"));
        });
    }

    public void GameResume(){
        menuPopup.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Inst.gamePaused = false;
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Default");
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #else
            Application.Quit();
        #endif
    }

    public void GameSave(){
        GameManager.Inst.isSave = true;
        
        PlayerPrefs.SetInt("isSave", 1);

        //cardHand 저장
        PlayerPrefs.SetInt("cardHandCount", CardManager.Inst.cardHand.Count);
        for(int i=0; i<CardManager.Inst.cardHand.Count; i++)
            PlayerPrefs.SetString("cardHand" + i.ToString(), CardManager.Inst.cardHand[i].item.name);
        
        //card buffer 저장
        PlayerPrefs.SetInt("itemBufferCount", CardManager.Inst.itemBuffer.Count);
        for(int i=0; i<CardManager.Inst.itemBuffer.Count; i++)
            PlayerPrefs.SetString("itemBuffer"+i.ToString(), CardManager.Inst.itemBuffer[i].name);
        
        //dump buffer 저장
        PlayerPrefs.SetInt("dumpBufferCount", CardManager.Inst.dumpBuffer.Count);
        for(int i=0; i<CardManager.Inst.dumpBuffer.Count; i++)
            PlayerPrefs.SetString("dumpBuffer"+i.ToString(), CardManager.Inst.dumpBuffer[i].name);

        //current request buffer 저장
        PlayerPrefs.SetInt("currentRequestBufferCount", RequestManager.Inst.currentRequestBuffer.Count);
        for(int i=0; i<RequestManager.Inst.currentRequestBuffer.Count; i++)
            PlayerPrefs.SetString("currentRequestBuffer"+i.ToString(), RequestManager.Inst.currentRequestBuffer[i].requestName);
        
        //gauge 저장
        PlayerPrefs.SetInt("faith", GaugeManager.Inst.faith);
        PlayerPrefs.SetInt("abundance", GaugeManager.Inst.abundance);
        PlayerPrefs.SetInt("refinement", GaugeManager.Inst.refinement);
        PlayerPrefs.SetFloat("maxAbundance", GaugeManager.Inst.maxAbundance);
        PlayerPrefs.SetFloat("maxRefinement", GaugeManager.Inst.maxRefinement);

        if (!savePopup.activeSelf){
            savePopup.SetActive(true);
            savePopup.transform.SetAsLastSibling();
        }
    }

    public void CloseSavePopup(){
        savePopup.SetActive(false);
    }
}
