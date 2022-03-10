using System;
using System.Collections;
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

        public void ResetAllDatas()
        {
            CurrentPlayerData.ResetPlayerData();
            CurrentPlayerBuyData.ResetPlayerBuyData();
            CurrentPlayerCustomizeItemOpenData.ResetPlayerCustomizeItemOpenData();
            CurrentPlayerCustomizeData.ResetPlayerCustomizeData();
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
                DontDestroyOnLoad(this.gameObject);
            }
            else if (dataMgr != this)
                Destroy(this.gameObject);
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
    }
}
