using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Capsule.Entity
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager dataMgr;
        public static DataManager Instance
        {
            get
            {
                if (dataMgr == null)
                    dataMgr = FindObjectOfType<DataManager>();
                return dataMgr;
            }
        }

        private void Awake()
        {
            if (dataMgr == null)
            {
                dataMgr = this;
                CurrentPlayerData = new PlayerData();
                CurrentPlayerBuyData = new PlayerBuyData();
                CurrentPlayerCustomizeItemOpenData = new PlayerCustomizeItemOpenData();
                CurrentPlayerCustomizeData = new PlayerCustomizeData();
                CurrentPlayerGameData = new PlayerGameData();
                CurrentPlayerStageClearData = new PlayerStageClearData();
                DontDestroyOnLoad(this.gameObject);
            }
            else if (dataMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            SetStageDatasPreviews();
            SetKindDatasPreviews();
        }

        public void SaveBeforeQuit()
        {
            CurrentPlayerData.SavePlayerData();
            CurrentPlayerCustomizeData.SavePlayerCustomizeData();
        }

        public void ResetAllDatas()
        {
            PlayerPrefs.SetInt("IsFirstPlayStage", 0);
            PlayerPrefs.SetInt("IsFirstPlayArcade", 0);
            PlayerPrefs.SetInt("IsArcadeModeOpen", 0);
            PlayerPrefs.SetInt("IsPracticeModeOpen", 0);
            PlayerPrefs.SetInt("IsBotModeOpen", 0);
            PlayerPrefs.SetInt("IsMultiPlayModeOpen", 0);
            CurrentPlayerData.ResetPlayerData();
            CurrentPlayerBuyData.ResetPlayerBuyData();
            CurrentPlayerCustomizeItemOpenData.ResetPlayerCustomizeItemOpenData();
            CurrentPlayerCustomizeData.ResetPlayerCustomizeData();
            CurrentPlayerGameData.ResetPlayerGameData();
            CurrentPlayerStageClearData.ResetPlayerStageClearData();
        }

        public List<CustomizingBodyData> customizingBodyDatas;
        public List<CustomizingHeadData> customizingHeadDatas;
        public List<CustomizingFaceData> customizingFaceDatas;
        public List<CustomizingGloveData> customizingGloveDatas;
        public List<CustomizingClothData> customizingClothDatas;
        public List<CustomizingPresetData> customizingPresetDatas;

        private PlayerData currentPlayerData = null;
        public PlayerData CurrentPlayerData
        {
            get { return currentPlayerData; }
            private set { currentPlayerData = value; }
        }
        private PlayerBuyData currentPlayerBuyData = null;
        private List<CustomizingBody> bodyBuyData;
        public List<CustomizingBody> BodyBuyData { get { return bodyBuyData; } }
        private List<CustomizingHead> headBuyData;
        public List<CustomizingHead> HeadBuyData { get { return headBuyData; } }
        private List<CustomizingFace> faceBuyData;
        public List<CustomizingFace> FaceBuyData { get { return faceBuyData; } }
        private List<CustomizingGlove> gloveBuyData;
        public List<CustomizingGlove> GloveBuyData { get { return gloveBuyData; } }
        private List<CustomizingCloth> clothBuyData;
        public List<CustomizingCloth> ClothBuyData { get { return clothBuyData; } }
        private List<CustomizingPreset> presetBuyData;
        public List<CustomizingPreset> PresetBuyData { get { return presetBuyData; } }
        public PlayerBuyData CurrentPlayerBuyData
        {
            get
            {
                if (currentPlayerBuyData == null)
                {
                    currentPlayerBuyData = new PlayerBuyData();
                    SetPlayerBuyData(currentPlayerBuyData);
                }
                return currentPlayerBuyData;
            }
            private set
            {
                currentPlayerBuyData = value;
                SetPlayerBuyData(value);
            }
        }
        private void SetPlayerBuyData(PlayerBuyData buyData)
        {
            bodyBuyData = new List<CustomizingBody>();
            headBuyData = new List<CustomizingHead>();
            faceBuyData = new List<CustomizingFace>();
            gloveBuyData = new List<CustomizingGlove>();
            clothBuyData = new List<CustomizingCloth>();
            presetBuyData = new List<CustomizingPreset>();

            foreach (String str in buyData.BuyData)
            {
                int position = str.IndexOf("=");
                if (position < 0)
                    continue;
                String dataNum = str.Substring(0, position);
                String dataType = str.Substring(position + 1);
                try
                {
                    if (dataType == ((int)CustomizingType.BODY).ToString())
                        bodyBuyData.Add((CustomizingBody)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.HEAD).ToString())
                        headBuyData.Add((CustomizingHead)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.FACE).ToString())
                        faceBuyData.Add((CustomizingFace)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.GLOVE).ToString())
                        gloveBuyData.Add((CustomizingGlove)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.CLOTH).ToString())
                        clothBuyData.Add((CustomizingCloth)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.PRESET).ToString())
                        presetBuyData.Add((CustomizingPreset)int.Parse(dataNum));
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        private PlayerCustomizeItemOpenData currentPlayerCustomizeItemOpenData = null;
        private List<CustomizingBody> bodyOpenData;
        public List<CustomizingBody> BodyOpenData { get { return bodyOpenData; } }
        private List<CustomizingHead> headOpenData;
        public List<CustomizingHead> HeadOpenData { get { return headOpenData; } }
        private List<CustomizingFace> faceOpenData;
        public List<CustomizingFace> FaceOpenData { get { return faceOpenData; } }
        private List<CustomizingGlove> gloveOpenData;
        public List<CustomizingGlove> GloveOpenData { get { return gloveOpenData; } }
        private List<CustomizingCloth> clothOpenData;
        public List<CustomizingCloth> ClothOpenData { get { return clothOpenData; } }

        public PlayerCustomizeItemOpenData CurrentPlayerCustomizeItemOpenData
        {
            get
            {
                if (currentPlayerCustomizeItemOpenData == null)
                {
                    currentPlayerCustomizeItemOpenData = new PlayerCustomizeItemOpenData();
                    SetPlayerCustomizeItemOpenData(currentPlayerCustomizeItemOpenData);
                }
                return currentPlayerCustomizeItemOpenData;
            }
            private set
            {
                currentPlayerCustomizeItemOpenData = value;
                SetPlayerCustomizeItemOpenData(value);
            }
        }
        private void SetPlayerCustomizeItemOpenData(PlayerCustomizeItemOpenData openData)
        {
            bodyOpenData = new List<CustomizingBody>();
            headOpenData = new List<CustomizingHead>();
            faceOpenData = new List<CustomizingFace>();
            gloveOpenData = new List<CustomizingGlove>();
            clothOpenData = new List<CustomizingCloth>();

            foreach (String str in openData.ItemOpenData)
            {
                int position = str.IndexOf("=");
                if (position < 0)
                    continue;
                String dataNum = str.Substring(0, position);
                String dataType = str.Substring(position + 1);
                try
                {
                    if (dataType == ((int)CustomizingType.BODY).ToString())
                        bodyOpenData.Add((CustomizingBody)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.HEAD).ToString())
                        headOpenData.Add((CustomizingHead)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.FACE).ToString())
                        faceOpenData.Add((CustomizingFace)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.GLOVE).ToString())
                        gloveOpenData.Add((CustomizingGlove)int.Parse(dataNum));
                    else if (dataType == ((int)CustomizingType.CLOTH).ToString())
                        clothOpenData.Add((CustomizingCloth)int.Parse(dataNum));
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        private PlayerCustomizeData currentPlayerCustomizeData = null;
        public PlayerCustomizeData CurrentPlayerCustomizeData
        {
            get { return currentPlayerCustomizeData; }
            private set { currentPlayerCustomizeData = value; }
        }

        public void UnlockAllCustomizeDatas()
        {
            CurrentPlayerBuyData.ResetPlayerBuyData();
            CurrentPlayerCustomizeItemOpenData.ResetPlayerCustomizeItemOpenData();
            bodyOpenData = new List<CustomizingBody>();
            headOpenData = new List<CustomizingHead>();
            faceOpenData = new List<CustomizingFace>();
            gloveOpenData = new List<CustomizingGlove>();
            clothOpenData = new List<CustomizingCloth>();
            bodyBuyData = new List<CustomizingBody>();
            headBuyData = new List<CustomizingHead>();
            faceBuyData = new List<CustomizingFace>();
            gloveBuyData = new List<CustomizingGlove>();
            clothBuyData = new List<CustomizingCloth>();
            presetBuyData = new List<CustomizingPreset>();

            foreach (CustomizingPresetData data in customizingPresetDatas)
            {
                currentPlayerBuyData.AddPlayerBuyData(
                    (int)data.presetNum,
                    (int)CustomizingType.PRESET);
                presetBuyData.Add(data.presetNum);
            }
            foreach (CustomizingBodyData data in customizingBodyDatas)
            {
                if (data.bodyNum != CustomizingBody.DEFAULT)
                {
                    currentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.bodyNum,
                        (int)CustomizingType.BODY);
                    bodyOpenData.Add(data.bodyNum);
                    bodyBuyData.Add(data.bodyNum);
                }
            }
            foreach (CustomizingHeadData data in customizingHeadDatas)
            {
                if (data.headNum != CustomizingHead.DEFAULT)
                {
                    currentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.headNum,
                        (int)CustomizingType.HEAD);
                    headOpenData.Add(data.headNum);
                    headBuyData.Add(data.headNum);
                }
            }
            foreach (CustomizingFaceData data in customizingFaceDatas)
            {
                if (data.faceNum != CustomizingFace.DEFAULT)
                {
                    currentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.faceNum,
                        (int)CustomizingType.FACE);
                    faceOpenData.Add(data.faceNum);
                    faceBuyData.Add(data.faceNum);
                }
            }
            foreach (CustomizingGloveData data in customizingGloveDatas)
            {
                if (data.gloveNum != CustomizingGlove.DEFAULT)
                {
                    currentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.gloveNum,
                        (int)CustomizingType.GLOVE);
                    gloveOpenData.Add(data.gloveNum);
                    gloveBuyData.Add(data.gloveNum);
                }
            }
            foreach (CustomizingClothData data in customizingClothDatas)
            {
                if (data.clothNum != CustomizingCloth.DEFAULT)
                {
                    currentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.clothNum,
                        (int)CustomizingType.CLOTH);
                    clothOpenData.Add(data.clothNum);
                    clothBuyData.Add(data.clothNum);
                }
            }
        }

        public CustomizingBodyData GetBodyData(CustomizingBody bodyNum)
        {
            var result = from data in customizingBodyDatas
                         where data.bodyNum == bodyNum
                         select data;
            if (result != null && result.Count<CustomizingBodyData>() > 0)
                return result.ElementAt<CustomizingBodyData>(0);
            else
                return null;
        }

        public CustomizingHeadData GetHeadData(CustomizingHead headNum)
        {
            var result = from data in customizingHeadDatas
                         where data.headNum == headNum
                         select data;
            if (result != null && result.Count<CustomizingHeadData>() > 0)
                return result.ElementAt<CustomizingHeadData>(0);
            else
                return null;
        }

        public CustomizingFaceData GetFaceData(CustomizingFace faceNum)
        {
            var result = from data in customizingFaceDatas
                         where data.faceNum == faceNum
                         select data;
            if (result != null && result.Count<CustomizingFaceData>() > 0)
                return result.ElementAt<CustomizingFaceData>(0);
            else
                return null;
        }

        public CustomizingGloveData GetGloveData(CustomizingGlove gloveNum)
        {
            var result = from data in customizingGloveDatas
                         where data.gloveNum == gloveNum
                         select data;
            if (result != null && result.Count<CustomizingGloveData>() > 0)
                return result.ElementAt<CustomizingGloveData>(0);
            else
                return null;
        }

        public CustomizingClothData GetClothData(CustomizingCloth clothNum)
        {
            var result = from data in customizingClothDatas
                         where data.clothNum == clothNum
                         select data;
            if (result != null && result.Count<CustomizingClothData>() > 0)
                return result.ElementAt<CustomizingClothData>(0);
            else
                return null;
        }

        public CustomizingPresetData GetPresetData(CustomizingPreset presetNum)
        {
            var result = from data in customizingPresetDatas
                         where data.presetNum == presetNum
                         select data;
            if (result != null && result.Count<CustomizingPresetData>() > 0)
                return result.ElementAt<CustomizingPresetData>(0);
            else
                return null;
        }

        private PlayerGameData currentPlayerGameData = null;
        public PlayerGameData CurrentPlayerGameData
        {
            get
            {
                if (currentPlayerGameData == null)
                    currentPlayerGameData = new PlayerGameData();
                return currentPlayerGameData;
            }
            private set { currentPlayerGameData = value; }
        }
        private PlayerStageClearData currentPlayerStageClearData = null;
        public PlayerStageClearData CurrentPlayerStageClearData
        {
            get
            {
                if (currentPlayerStageClearData == null)
                    currentPlayerStageClearData = new PlayerStageClearData();
                return currentPlayerStageClearData;
            }
            private set { currentPlayerStageClearData = value; }
        }

        public List<GameModeData> gameModeDatas = new List<GameModeData>()
        {
            new GameModeData()
            {
                mode = GameMode.ARCADE,
                name = "아케이드 모드",
                desc = "최고 스코어에 도전하자!"
            },
            new GameModeData()
            {
                mode = GameMode.STAGE,
                name = "스테이지 모드",
                desc = "스테이지 클리어에 도전하자!"
            },
            new GameModeData()
            {
                mode = GameMode.PRACTICE,
                name = "연습 모드",
                desc = "자유롭게 연습해서 실력을 키우자!"
            },
            new GameModeData()
            {
                mode = GameMode.BOT,
                name = "AI봇 대전 모드",
                desc = "AI봇과 대전하며 실력을 키우자!"
            },
            new GameModeData()
            {
                mode = GameMode.MULTI,
                name = "랜덤 대전 모드",
                desc = "랜덤으로 매칭되는 게임을 즐기자!"
            },
            new GameModeData()
            {
                mode = GameMode.RANK,
                name = "랭크 대전 모드",
                desc = "점수를 걸고 실력을 겨루자!"
            },
            new GameModeData()
            {
                mode = GameMode.CUSTOM,
                name = "사용자 설정 모드",
                desc = "자유롭게 설정하여 즐기자!"
            },
        };

        public List<Sprite> gameKindSprites = new List<Sprite>();
        private List<GameKindData> gameKindDatas = new List<GameKindData>()
        {
            new GameKindData ()
            {
                kind = GameKind.GOAL_IN,
                name = "공 굴려서 골인~!",
                desc = "골대까지 공을 굴려가자!!\n\n무시무시한 장애물들을 피해야해!\n\n점프와 다이브를 적극 활용하자!"
            },
            new GameKindData ()
            {
                kind = GameKind.BATTLE_ROYAL,
                name = "공 위에서 배틀로얄!",
                desc = "마지막 까지 공위에 있는 캡슐이 승자!!\n\n눈치 보며 도망다니자구~\n\n다른 캡슐을 다이브로 쓰러뜨리자!"
            },
            new GameKindData ()
            {
                kind = GameKind.RACING,
                name = "내 공이 제일 빠르다구~!",
                desc = "누가 더 빠른지 승부~!\n\n빠른 속도로 최고 기록을 갱신하자~\n\n장애물을 잘 피해야해~!"
            },
            new GameKindData()
            {
                kind = GameKind.UP_UP,
                name = "점프로 업~ 업~!",
                desc = "누가 더 많이 올라가나 내기~\n\n점프를 적극 활용해서\n더 높이 올라가자~!!"
            },
            new GameKindData()
            {
                kind = GameKind.NEXT_TARGET,
                name = "다음 타겟은?? 너라공~!",
                desc = "모두 타겟을 노려야해!!\n\n시간이 다 지날 때까지 못잡으면 진다구~\n\n타겟이 되면 멀리 도망가자~!"
            },
        };

        public List<GameKindData> GameKindDatas
        {
            get { return gameKindDatas; }
        }
        private void SetKindDatasPreviews()
        {
            if (gameKindSprites.Count > 0)
            {
                foreach (GameKindData kindData in gameKindDatas)
                {
                    if (kindData.preview == null)
                        kindData.preview = gameKindSprites[(int)kindData.kind];
                }
            }
        }

        public List<Sprite> rewardSprites = new List<Sprite>();
        private void SetStageDatasPreviews()
        {
            foreach (GameStageData stageData in gameStageDatas)
            {
                if (stageData.rewards != null && stageData.rewards.Count > 0)
                {
                    foreach (RewardData rewardData in stageData.rewards)
                        rewardData.SetRewardPreview();
                }
            }
        }

        private List<GameStageData> gameStageDatas = new List<GameStageData>()
        {
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.TUTORIAL_1,
                name = "튜토리얼 1",
                desc = "[공 굴려서 골인~!]의 기본적인 조작을 배워보자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 100,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 10,
                        onlyOnce = false,
                    }
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.STAGE_1,
                name = "스테이지 1",
                desc = "장애물을 돌파해 클리어하자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 200,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 30,
                        onlyOnce = false,
                    }
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.TUTORIAL_2,
                name = "튜토리얼 2",
                desc = "[공 굴려서 골인~!]의 [점프] 조작을 배워보자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 200,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 20,
                        onlyOnce = false,
                    }
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.STAGE_2,
                name = "스테이지 2",
                desc = "[점프]를 적극 활용해 장애물을 돌파하자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 500,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 50,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.CUSTOMIZING_FACE,
                        amount = (int)CustomizingFace.ROBO,
                        onlyOnce = true,
                    },
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.TUTORIAL_3,
                name = "튜토리얼 3",
                desc = "[공 굴려서 골인~!]의 [다이브] 조작을 배워보자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 200,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 30,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.GAME_MODE,
                        amount = 0,
                        onlyOnce = true,
                    }
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.STAGE_3,
                name = "스테이지 3",
                desc = "[다이브]를 적극 활용해 장애물을 돌파하자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 1000,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 100,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.CUSTOMIZING_HEAD,
                        amount = (int)CustomizingHead.ROBO,
                        onlyOnce = true,
                    },
                },
            },
            new GameStageData()
            {
                kind = GameKind.GOAL_IN,
                stage = GameStage.STAGE_4,
                name = "스테이지 4",
                desc = "지금까지 배운걸 최대한 활용해보자!",
                rewards = new List<RewardData>()
                {
                    new RewardData()
                    {
                        kind = RewardKind.COIN,
                        amount = 2000,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.EXP,
                        amount = 150,
                        onlyOnce = false,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.CUSTOMIZING_CLOTH,
                        amount = (int)CustomizingCloth.COMMON_1,
                        onlyOnce = true,
                    },
                    new RewardData()
                    {
                        kind = RewardKind.GAME_MODE,
                        amount = 1,
                        onlyOnce= true,
                    },
                },
            },
        };
        public List<GameStageData> GameStageDatas
        {
            get { return gameStageDatas; }
            private set { gameStageDatas = value; }
        }

        public bool HasNextStage()
        {
            return HasNextStage(CurrentGameData.Stage);
        }

        public bool HasNextStage(int stageNum)
        {
            return stageNum + 1 < (int)GameStage.STAGE_ALL_CLEAR;
        }

        public bool HasNextStage(GameStage stage)
        {
            return (int)stage + 1 < (int)GameStage.STAGE_ALL_CLEAR;
        }

        public GameStageData GetNextStage()
        {
            return GetNextStage(CurrentPlayerGameData.HighestStage);
        }

        public GameStageData GetNextStage(int stage)
        {
            int nextStageNum = stage + 1;
            if (!HasNextStage(stage))
                nextStageNum--;
            return gameStageDatas[nextStageNum];
        }

        public GameStageData GetNextStage(GameStage stage)
        {
            return GetNextStage((int)stage);
        }

        public string GetNextStageString()
        {
            return GetNextStage(currentPlayerGameData.HighestStage).name;
        }

        public string GetCurrentStageString()
        {
            return gameStageDatas[(int)CurrentGameData.Stage].name;
        }

        public string GetHighestStageString()
        {
            if (currentPlayerGameData.HighestStage == -1)
                return "없음";
            return gameStageDatas[currentPlayerGameData.HighestStage].name;
            //return GetStageString((GameStage)currentPlayerGameData.HighestStage);
        }

        public void OpenArcadeMode()
        {
            PlayerPrefs.SetInt("IsArcadeModeOpen", 1);
        }

        public void OpenPracticeMode()
        {
            PlayerPrefs.SetInt("IsPracticeModeOpen", 1);
        }

        public void OpenBotMode()
        {
            PlayerPrefs.SetInt("IsBotModeOpen", 1);
        }

        public GameData CurrentGameData { get; set; }
    }
}
