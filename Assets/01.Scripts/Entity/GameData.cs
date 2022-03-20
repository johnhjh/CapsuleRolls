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
        TUTORIAL_0 = 0,
        TUTORIAL_1,
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
}

