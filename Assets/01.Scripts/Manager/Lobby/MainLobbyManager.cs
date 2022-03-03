using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;
using Capsule.Player;

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
        PlayerTransform.Instance.SetPosition(new Vector3(2.29f, -0.17f, -5.4f));
        PlayerTransform.Instance.SetRotation(Quaternion.Euler(19.94f, 202f, -4.7f));
        //PlayerTransform.Instance.SetScale(new Vector3(1.18f, 1.18f, 1.18f));
        PlayerTransform.Instance.SetScale(1.18f);
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
