using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;

public class SoloPlayManager : MonoBehaviour
{
    private static SoloPlayManager soloPlayMgr;
    public static SoloPlayManager Instance
    {
        get
        {
            if (soloPlayMgr == null)
                soloPlayMgr = FindObjectOfType<SoloPlayManager>();
            return soloPlayMgr;
        }
    }

    private void Awake()
    {
        if (soloPlayMgr == null)
            soloPlayMgr = this;
        else if (soloPlayMgr != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        BGMManager.Instance.ChangeBGM(BGMType.BATTLE);
        SFXManager.Instance.PlayOneShotSFX(SFXType.LOAD_DONE);
        SceneLoadManager.Instance.CurrentScene = LobbySceneType.SOLO;
    }

    public void BackToMainLobby()
    {
        SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
        StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
    }
}
