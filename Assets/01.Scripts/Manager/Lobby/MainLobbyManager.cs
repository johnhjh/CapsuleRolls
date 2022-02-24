using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;

public class MainLobbyManager : MonoBehaviour
{
    private static MainLobbyManager mainLobbyMgr;

    public static MainLobbyManager Instance
    {
        get
        {
            if (mainLobbyMgr == null)
                mainLobbyMgr = FindObjectOfType<MainLobbyManager>();
            return mainLobbyMgr;
        }
    }

    private void Awake()
    {
        if (mainLobbyMgr == null)
            mainLobbyMgr = this;
        else if (mainLobbyMgr != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        BGMManager.Instance.ChangeBGM(BGMType.MAIN);
        SceneLoadManager.Instance.CurrentScene = LobbySceneType.MAIN_LOBBY;
    }

    public void MoveToCustomizeScene()
    {
        StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.CUSTOMIZE, true));
    }

    public void MenuClick()
    {
        SFXManager.Instance.PlaySFX(SFXEnum.OK);
    }
}
