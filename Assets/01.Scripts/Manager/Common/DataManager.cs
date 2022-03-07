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

        private void Awake()
        {
            if (dataMgr == null)
            {
                dataMgr = this;
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
    }
}
