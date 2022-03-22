using UnityEngine;
namespace Capsule.Entity
{
    public enum GameMode
    {
        ARCADE = 0,
        STAGE,
        PRACTICE,
        BOT,
        MULTI,
        RANK,
        CUSTOM,
    }

    public enum GameKind
    {
        ROLL_THE_BALL = 0,
        THROWING_FEEDER,
        ATTACK_INVADER,
    }

    public enum GameStage
    {
        TUTORIAL_1 = 0,
        STAGE_1,
        TUTORIAL_2,
        STAGE_2,
        TUTORIAL_3,
        STAGE_3,
        STAGE_4,
        STAGE_ALL_CLEAR,
        STAGE_5,
        STAGE_6,
        STAGE_7,
        STAGE_8,
        STAGE_9,
        STAGE_10,
    }

    public enum RewardKind
    {
        COIN = 0,
        EXP,
        CUSTOMIZING_BODY,
        CUSTOMIZING_HEAD,
        CUSTOMIZING_FACE,
        CUSTOMIZING_GLOVE,
        CUSTOMIZING_CLOTH,
    }

    public enum GameMap
    {
        CUSHION = 0,
        BEACH,
        LARVA,
    }

    public enum AIDifficulty
    {
        EASY = 0,
        MEDIUM,
        HARD,
    }

    public class GameData
    {
        private GameMode mode;
        public GameMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private GameStage stage;
        public GameStage Stage
        {
            get { return stage; }
            set { stage = value; }
        }

        private GameKind kind;
        public GameKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        private GameMap map;
        public GameMap Map
        {
            get { return map; }
            set { map = value; }
        }

        private AIDifficulty difficulty;
        public AIDifficulty Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }
    }

    public class GameModeData
    {
        public GameMode mode;
        public string name;
        public string desc;
    }

    [System.Serializable]
    public class GameKindData
    {
        public GameKind kind;
        public Sprite preview;
        public string desc;
    }

    [System.Serializable]
    public class GameStageData
    {
        public GameKind kind;
        public GameStage stage;
        public string name;
        public string desc;
        public RewardData[] rewards;
    }

    [System.Serializable]
    public class RewardData
    {
        public RewardKind kind;
        public int amount;
        public Sprite preview;
        public bool onlyOnce;
    }
}

