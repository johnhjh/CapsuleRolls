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

        public void ResetAllDatas()
        {
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
            get { return currentPlayerBuyData; }
            private set
            {
                currentPlayerBuyData = value;
                bodyBuyData = new List<CustomizingBody>();
                headBuyData = new List<CustomizingHead>();
                faceBuyData = new List<CustomizingFace>();
                gloveBuyData = new List<CustomizingGlove>();
                clothBuyData = new List<CustomizingCloth>();
                presetBuyData = new List<CustomizingPreset>();

                foreach (String str in value.BuyData)
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
            get { return currentPlayerCustomizeItemOpenData; }
            private set
            {
                currentPlayerCustomizeItemOpenData = value;
                bodyOpenData = new List<CustomizingBody>();
                headOpenData = new List<CustomizingHead>();
                faceOpenData = new List<CustomizingFace>();
                gloveOpenData = new List<CustomizingGlove>();
                clothOpenData = new List<CustomizingCloth>();

                foreach (String str in value.ItemOpenData)
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
            get { return currentPlayerGameData; }
            private set { currentPlayerGameData = value; }
        }
        private PlayerStageClearData currentPlayerStageClearData = null;
        public PlayerStageClearData CurrentPlayerStageClearData
        {
            get { return currentPlayerStageClearData; }
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

        public List<GameKindData> gameKindDatas = new List<GameKindData>()
        {
            new GameKindData ()
            {
                kind = GameKind.ROLL_THE_BALL,
                preview = null,
                name = "공 굴려서 골인~!",
                desc = "골을 더 많이 넣으면 승리!!\n\n장애물을 피해 골대까지 굴러가자!\n\n점프로 상대팀을 저지하자!"
            },
            new GameKindData ()
            {
                kind = GameKind.THROWING_FEEDER,
                preview = null,
                name = "먹이를 던져주자~!",
                desc = "먹이를 더 많이 먹이면 승리!!\n\n과일이 아니면 싫어한다구!\n\n과일이 아니면 상대팀한테 던지자!"
            },
            new GameKindData ()
            {
                kind = GameKind.ATTACK_INVADER,
                preview = null,
                name = "침략자를 막자~!",
                desc = "침략자를 더 많이 막으면 승리!!\n\n무기는 죽을 때 마다 계속 바뀐다구!\n\n상대팀도 공격해서 방해하자!"
            }
        };

        private List<GameStageData> gameStageDatas = new List<GameStageData>()
        {
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.TUTORIAL_1,
                name = "튜토리얼 1",
                desc = "[공 굴려서 골인~!]의 기본적인 조작을 배워보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.STAGE_1,
                name = "스테이지 1",
                desc = "장애물을 돌파해 클리어하자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.TUTORIAL_2,
                name = "튜토리얼 2",
                desc = "[공 굴려서 골인~!]의 특별한 조작을 배워보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.STAGE_2,
                name = "스테이지 2",
                desc = "장애물을 돌파해 클리어하자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.TUTORIAL_3,
                name = "튜토리얼 3",
                desc = "[공 굴려서 골인~!]의 도움이 될 테크닉을 배워보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.STAGE_3,
                name = "스테이지 3",
                desc = "지금까지 배운걸 최대한 활용해보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ROLL_THE_BALL,
                stage = GameStage.STAGE_4,
                name = "스테이지 4",
                desc = "지금까지 배운걸 최대한 활용해보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ATTACK_INVADER,
                stage = GameStage.STAGE_5,
                name = "스테이지 5",
                desc = "[CapsuleFights]의 기본적인 조작을 배워보자!",
            },
            new GameStageData()
            {
                kind = GameKind.ATTACK_INVADER,
                stage = GameStage.STAGE_6,
                name = "스테이지 6",
            },
            new GameStageData()
            {
                kind = GameKind.ATTACK_INVADER,
                stage = GameStage.STAGE_7,
                name = "스테이지 7",
            },
            new GameStageData()
            {
                kind = GameKind.ATTACK_INVADER,
                stage = GameStage.STAGE_8,
                name = "스테이지 8",
            },
            new GameStageData()
            {
                stage = GameStage.STAGE_9,
                name = "스테이지 9",
            },
            new GameStageData()
            {
                stage = GameStage.STAGE_10,
                name = "스테이지 10",
            },
        };
        public List<GameStageData> GameStageDatas
        {
            get { return gameStageDatas; }
            private set { gameStageDatas = value; }
        }

        public GameStageData GetNextStage(int stage)
        {
            int nextStageNum = stage + 1;
            if ((GameStage)nextStageNum == GameStage.STAGE_ALL_CLEAR)
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

        public string GetHighestStageString()
        {
            if (currentPlayerGameData.HighestStage == -1)
                return "없음";
            return gameStageDatas[currentPlayerGameData.HighestStage].name;
            //return GetStageString((GameStage)currentPlayerGameData.HighestStage);
        }
        /*
        public string GetStageString(GameStage stageNum)
        {
            switch (stageNum)
            {
                case GameStage.TUTORIAL_1:
                case GameStage.TUTORIAL_2:
                case GameStage.TUTORIAL_3:
                    return "튜토리얼";
                case GameStage.STAGE_1:
                    return "스테이지 1";
                case GameStage.STAGE_2:
                    return "스테이지 2";
                case GameStage.STAGE_3:
                    return "스테이지 3";
                case GameStage.STAGE_4:
                    return "스테이지 4";
                case GameStage.STAGE_5:
                    return "스테이지 5";
                case GameStage.STAGE_6:
                    return "스테이지 6";
                case GameStage.STAGE_7:
                    return "스테이지 7";
                case GameStage.STAGE_8:
                    return "스테이지 8";
                case GameStage.STAGE_9:
                    return "스테이지 9";
                case GameStage.STAGE_10:
                    return "스테이지 10";
                default:
                    return "튜토리얼";
            }
        }
        */

        public GameData CurrentGameData { get; set; }
    }
}
