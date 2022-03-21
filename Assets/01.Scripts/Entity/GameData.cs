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
        TUTORIAL_2,
        TUTORIAL_3,
        STAGE_1,
        STAGE_2,
        STAGE_3,
        STAGE_4,
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
        CUSTOMIZING_BODY,
        CUSTOMIZING_HEAD,
        CUSTOMIZING_FACE,
        CUSTOMIZING_GLOVE,
        CUSTOMIZING_CLOTH,
    }

    public enum GameMap
    {
        CUSHION = 0,
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

        private GameKind kind;
        public GameKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        private GameMap stage;
        public GameMap Stage
        {
            get { return stage; }
            set { stage = value; }
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
        public GameStage stage;
        public Sprite preview;
        public string name;
        public string desc;
    }

    [System.Serializable]
    public class RewardData
    {
        public RewardKind kind;
        public int reward;
        public Sprite preview;
    }
}

