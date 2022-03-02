using System.Collections;
using UnityEngine;

namespace Capsule.Entity
{
    public enum CustomizingBody
    {
        DEFAULT = 0,
        PINK,
        BLUE,
        GREEN,
        YELLOW,
        BLACK,
        GOLDEN,
        SILVER,
        TRANSPARENT,
    }

    public enum CustomizingHead
    {
        DEFAULT = 0,
        COMMON_1,
        COMMON_2,
        COMMON_3,
        COMMON_4,
        COMMON_5,
        COMMON_6,
        RARE_1,
        RARE_2,
        RARE_3,
        RARE_4,
        RARE_5,
        RARE_6,
        RARE_7,
        RARE_8,
        EPIC_1,
        EPIC_2,
        EPIC_3,
        EPIC_4,
        EPIC_5,
        EPIC_6,
        EPIC_7,
    }

    public enum CustomizingFace
    {
        DEFAULT = 0,
        COMMON_1,
        COMMON_2,
        COMMON_3,
        COMMON_4,
        COMMON_5,
        RARE_1,
        RARE_2,
        RARE_3,
        RARE_4,
        EPIC_1,
        EPIC_2,
        EPIC_3,
    }

    public enum CustomizingGlove
    {
        DEFAULT = 0,

    }

    public enum CustomizingRarity
    {
        DEFAULT = 0,
        COMMON,
        RARE,
        EPIC,
        LEGEND,
    }

    public enum CustomizingType
    {
        BODY = 0,
        HEAD,
        FACE,
        GLOVE,
    }

    [System.Serializable]
    public class CustomizingData
    {
        public CustomizingType type;
        public CustomizingRarity rarity;
        public Sprite preview;
    }

    [System.Serializable]
    public class CustomizingBodyData : CustomizingData
    {
        public CustomizingBody bodyNum;
        public Material bodyMaterial;
    }

    [System.Serializable]
    public class CustomizingHeadData : CustomizingData
    {
        public CustomizingHead headNum;
        public GameObject headItem;
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    public class CustomizingFaceData : CustomizingData
    {
        public CustomizingFace faceNum;
        public GameObject faceItem;
        public Vector3 position;
        public Vector3 rotation;
    }
}