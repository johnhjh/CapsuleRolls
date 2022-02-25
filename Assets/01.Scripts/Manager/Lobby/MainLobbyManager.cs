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
        SFXManager.Instance.PlayOneShotSFX(SFXType.LOAD_DONE);
        SceneLoadManager.Instance.CurrentScene = LobbySceneType.MAIN_LOBBY;
    }

    public void MoveToCustomizeScene()
    {
        MoveToScene(LobbySceneType.CUSTOMIZE);
    }

    public void MoveToSoloPlayScene()
    {
        MoveToScene(LobbySceneType.SOLO);
    }

    public void MoveToMultiPlayScene()
    {
        MoveToScene(LobbySceneType.MULTI);
    }

    public void MoveToShoppingScene()
    {
        MoveToScene(LobbySceneType.SHOPPING);
    }

    public void MoveToScene(LobbySceneType sceneType)
    {
        StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(sceneType, true));
    }

    public void MenuClick()
    {
        SFXManager.Instance.PlayOneShotSFX(SFXType.OK);
    }
}
