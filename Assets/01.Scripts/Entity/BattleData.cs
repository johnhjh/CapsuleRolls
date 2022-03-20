namespace Capsule.Entity
{
    public enum BattleItemOption
    {
        NONE = 0,
        NORMAL,
        DYNAMIC,
    }

    public class BattleData
    {
        private int roundCount;
        public int RoundCount
        {
            get { return roundCount; }
            set { roundCount = value; }
        }

        private bool usingTimeLimit;
        public bool UsingTimeLimit
        {
            get { return usingTimeLimit; }
            set { usingTimeLimit = value; }
        }

        private int roundTime;
        public int RoundTime
        {
            get { return roundTime; }
            set { roundTime = value; }
        }

        private bool usingWinScore;
        public bool UsingWinScore
        {
            get { return usingWinScore; }
            set { usingWinScore = value; }
        }

        private int winScore;
        public int WinScore
        {
            get { return winScore; }
            set { winScore = value; }
        }

        private BattleItemOption itemOption;
        public BattleItemOption ItemOption
        {
            get { return itemOption; }
            set { itemOption = value; }
        }
    }
}
