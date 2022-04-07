using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Entity
{
    public enum LevelExp
    {
        LEVEL_1 = 0,
        LEVEL_2 = 100,
        LEVEL_3 = 200,
        LEVEL_4 = 400,
        LEVEL_5 = 800,
        LEVEL_6 = 1600,
        LEVEL_7 = 3200,
        LEVEL_8 = 6400,
        LEVEL_9 = 12800,
        LEVEL_10 = 25600,
        LEVEL_11 = 51200,
        LEVEL_12 = 102400,
        LEVEL_MAX = 204800,
    }

    public static class LevelExpCalc
    {
        private readonly static List<int> levelExpList;

        static LevelExpCalc()
        {
            levelExpList = new List<int>
            {
                (int)LevelExp.LEVEL_1,
                (int)LevelExp.LEVEL_2,
                (int)LevelExp.LEVEL_3,
                (int)LevelExp.LEVEL_4,
                (int)LevelExp.LEVEL_5,
                (int)LevelExp.LEVEL_6,
                (int)LevelExp.LEVEL_7,
                (int)LevelExp.LEVEL_8,
                (int)LevelExp.LEVEL_9,
                (int)LevelExp.LEVEL_10,
                (int)LevelExp.LEVEL_11,
                (int)LevelExp.LEVEL_12
            };
        }

        public static int GetExpData(int level)
        {
            if (levelExpList != null && levelExpList.Count > level)
                return levelExpList[level - 1];
            else
                return (int)LevelExp.LEVEL_MAX;
        }
    }

    public class PlayerData
    {
        private string playerID;
        private string playerNickName;
        private int playerLevel;
        private int playerExp;
        private int playerCoin;
        private int playerRate;

        public PlayerData()
        {
            LoadPlayerData();
        }

        public PlayerData(string id, string nick, int level, int exp, int coin, int rating)
        {
            ID = id;
            NickName = nick;
            Level = level;
            Exp = exp;
            Coin = coin;
            Rating = rating;
        }

        public string ID
        {
            get { return playerID; }
            private set { playerID = value; }
        }
        public string NickName
        {
            get { return playerNickName; }
            set
            {
                playerNickName = value;
                PlayerPrefs.SetString("PlayerNick", value);
            }
        }
        public int Exp
        {
            get { return playerExp; }
            private set { playerExp = value; }
        }
        public int Level
        {
            get { return playerLevel; }
            private set { playerLevel = value; }
        }
        public int Coin
        {
            get { return playerCoin; }
            private set { playerCoin = value; }
        }
        public int Rating
        {
            get { return playerRate; }
            private set { playerRate = value; }
        }

        public void AddExp(int exp)
        {
            Exp += exp;
            PlayerPrefs.SetInt("PlayerExp", Exp);
            if (CheckLevelUP())
                LevelUP();
        }

        public void ResetLevel()
        {
            Level = 1;
            Exp = 0;
            PlayerPrefs.SetInt("PlayerLevel", Level);
            PlayerPrefs.SetInt("PlayerExp", Exp);
        }

        private void LevelUP()
        {
            Level += 1;
            Exp -= LevelExpCalc.GetExpData(Level);
            PlayerPrefs.SetInt("PlayerLevel", Level);
            PlayerPrefs.SetInt("PlayerExp", Exp);
            if (CheckLevelUP())
                LevelUP();
        }

        private bool CheckLevelUP()
        {
            return Exp <= (int)LevelExp.LEVEL_MAX &&
                Exp >= LevelExpCalc.GetExpData(Level + 1);
        }

        public void EarnCoin(int amount)
        {
            Coin += amount;
            PlayerPrefs.SetInt("PlayerCoins", Coin);
        }

        public void UseCoin(int amount)
        {
            Coin -= amount;
            PlayerPrefs.SetInt("PlayerCoins", Coin);
        }

        public void CalcRating(int amount)
        {
            Rating += amount;
            PlayerPrefs.SetInt("PlayerRating", Rating);
        }

        public void SavePlayerData()
        {
            PlayerPrefs.SetString("PlayerNick", NickName);
            PlayerPrefs.SetInt("PlayerExp", Exp);
            PlayerPrefs.SetInt("PlayerLevel", Level);
            PlayerPrefs.SetInt("PlayerCoins", Coin);
            PlayerPrefs.SetInt("PlayerRating", Rating);
        }

        private void LoadPlayerData()
        {
            ID = PlayerPrefs.GetString("PlayerID", "Guest" + Random.Range(1, 999).ToString("000"));
            NickName = PlayerPrefs.GetString("PlayerNick", "캡슐롤린이");
            Exp = PlayerPrefs.GetInt("PlayerExp", 0);
            Level = PlayerPrefs.GetInt("PlayerLevel", 1);
            Coin = PlayerPrefs.GetInt("PlayerCoins", 30000);
            Rating = PlayerPrefs.GetInt("PlayerRating", 1200);
        }

        public void ResetPlayerData()
        {
            PlayerPrefs.SetString("PlayerNick", "캡슐롤린이");
            PlayerPrefs.SetInt("PlayerExp", 0);
            PlayerPrefs.SetInt("PlayerLevel", 1);
            PlayerPrefs.SetInt("PlayerCoins", 30000);
            PlayerPrefs.SetInt("PlayerRating", 1200);
        }
    }

    public class PlayerCustomizeData
    {
        private int body;
        private int head;
        private int face;
        private int glove;
        private int cloth;

        public int Body
        {
            get { return body; }
            set { body = value; }
        }
        public int Head
        {
            get { return head; }
            set { head = value; }
        }
        public int Face
        {
            get { return face; }
            set { face = value; }
        }
        public int Glove
        {
            get { return glove; }
            set { glove = value; }
        }
        public int Cloth
        {
            get { return cloth; }
            set { cloth = value; }
        }

        public PlayerCustomizeData()
        {
            Body = PlayerPrefs.GetInt("CustomizeBody", 0);
            Head = PlayerPrefs.GetInt("CustomizeHead", 0);
            Face = PlayerPrefs.GetInt("CustomizeFace", 0);
            Glove = PlayerPrefs.GetInt("CustomizeGlove", 0);
            Cloth = PlayerPrefs.GetInt("CustomizeCloth", 0);
        }

        public PlayerCustomizeData(int body, int head, int face, int glove, int cloth)
        {
            Body = body;
            Head = head;
            Face = face;
            Glove = glove;
            Cloth = cloth;
        }

        public void SavePlayerCustomizeData()
        {
            PlayerPrefs.SetInt("CustomizeBody", Body);
            PlayerPrefs.SetInt("CustomizeHead", Head);
            PlayerPrefs.SetInt("CustomizeFace", Face);
            PlayerPrefs.SetInt("CustomizeGlove", Glove);
            PlayerPrefs.SetInt("CustomizeCloth", Cloth);
        }

        public void ResetPlayerCustomizeData()
        {
            PlayerPrefs.SetInt("CustomizeBody", 0);
            PlayerPrefs.SetInt("CustomizeHead", 0);
            PlayerPrefs.SetInt("CustomizeFace", 0);
            PlayerPrefs.SetInt("CustomizeGlove", 0);
            PlayerPrefs.SetInt("CustomizeCloth", 0);
        }
    }

    public class PlayerCustomizeItemOpenData
    {
        private List<string> playerCustomizeItemOpenData;

        public List<string> ItemOpenData
        {
            get { return playerCustomizeItemOpenData; }
            private set { playerCustomizeItemOpenData = value; }
        }

        public PlayerCustomizeItemOpenData()
        {
            LoadPlayerCustomizeItemOpenData();
        }

        private void LoadPlayerCustomizeItemOpenData()
        {
            ItemOpenData = new List<string>();
            int counter = 0;
            while (true)
            {
                string openData = PlayerPrefs.GetString("OpenData" + counter.ToString(), "");
                if (openData == "") break;
                playerCustomizeItemOpenData.Add(openData);
                counter++;
                InfiniteLoopDetector.Run();
            }
        }

        public void AddPlayerCustomizeItemOpenData(int data, int type)
        {
            string dataType = data.ToString() + "=" + type.ToString();
            PlayerPrefs.SetString("OpenData" + playerCustomizeItemOpenData.Count, dataType);
            playerCustomizeItemOpenData.Add(dataType);
        }

        public void ResetPlayerCustomizeItemOpenData()
        {
            int counter = 0;
            while (true)
            {
                string openData = PlayerPrefs.GetString("OpenData" + counter.ToString(), "");
                if (openData == "") break;
                PlayerPrefs.SetString("OpenData" + counter.ToString(), "");
                counter++;
                InfiniteLoopDetector.Run();
            }
            ItemOpenData = new List<string>();
        }
    }

    public class PlayerBuyData
    {
        private List<string> playerBuyData;

        public List<string> BuyData
        {
            get { return playerBuyData; }
            private set { playerBuyData = value; }
        }

        public PlayerBuyData()
        {
            LoadPlayerBuyData();
        }

        private void LoadPlayerBuyData()
        {
            BuyData = new List<string>();
            int counter = 0;
            while (true)
            {
                string buyData = PlayerPrefs.GetString("BuyData" + counter.ToString(), "");
                if (buyData == "") break;
                playerBuyData.Add(buyData);
                counter++;
                InfiniteLoopDetector.Run();
            }
        }

        public void AddPlayerBuyData(int data, int type)
        {
            string dataType = data.ToString() + "=" + type.ToString();
            PlayerPrefs.SetString("BuyData" + playerBuyData.Count, dataType);
            playerBuyData.Add(dataType);
        }

        public void ResetPlayerBuyData()
        {
            int counter = 0;
            while (true)
            {
                string buyData = PlayerPrefs.GetString("BuyData" + counter.ToString(), "");
                if (buyData == "") break;
                PlayerPrefs.SetString("BuyData" + counter.ToString(), "");
                counter++;
                InfiniteLoopDetector.Run();
            }
            BuyData = new List<string>();
        }
    }

    public class PlayerStageClearData
    {
        private List<bool> playerStageClearData;
        public List<bool> ClearData
        {
            get { return playerStageClearData; }
            private set { playerStageClearData = value; }
        }
        public PlayerStageClearData()
        {
            LoadPlayerStageClearData();
        }

        private void LoadPlayerStageClearData()
        {
            ClearData = new List<bool>();
            for (int i = 0; i < (int)GameStage.STAGE_ALL_CLEAR; i++)
            {
                if (PlayerPrefs.GetInt("Stage" + i.ToString(), 0) == 0)
                    ClearData.Add(false);
                else
                    ClearData.Add(true);
            }
        }

        public void StageClear(int stageNum)
        {
            string savedStage = "Stage" + stageNum.ToString();
            PlayerPrefs.SetInt(savedStage, PlayerPrefs.GetInt(savedStage, 0) + 1);
            if (ClearData.Count > stageNum)
                ClearData[stageNum] = true;
            if (stageNum > PlayerPrefs.GetInt("HighestStage", 0))
                PlayerPrefs.SetInt("HighestStage", stageNum);
        }

        public void StageClear(GameStage stage)
        {
            StageClear(((int)stage));
        }

        public void ResetPlayerStageClearData()
        {
            for (int i = 0; i < ClearData.Count; i++)
            {
                PlayerPrefs.SetInt("Stage" + i.ToString(), 0);
                ClearData[i] = false;
            }
            PlayerPrefs.SetInt("HighestStage", -1);
        }

        public void UnlockAllStages()
        {
            ClearData.Clear();
            for (int i = 0; i < (int)GameStage.STAGE_ALL_CLEAR; i++)
            {
                PlayerPrefs.SetInt("Stage" + i.ToString(), 1);
                ClearData.Add(true);
            }
        }
    }

    public class PlayerGameData
    {
        private int highestScore;
        private int highestStage;
        private int currentWins;
        private int mostWins;
        private int multiVictoryCount;
        private int multiPlayCount;
        private int totalPlayCount;
        private int totalKillCount;
        private int totalDeathCount;
        private int multiMostKillCount;

        public int HighestScore
        {
            get { return highestScore; }
            private set
            {
                highestScore = value;
                PlayerPrefs.SetInt("HighestScore", value);
            }
        }
        public int HighestStage
        {
            get { return highestStage; }
            private set
            {
                highestStage = value;
            }
        }
        public int CurrentWins
        {
            get { return currentWins; }
            private set
            {
                currentWins = value;
                PlayerPrefs.SetInt("CurrentWins", value);
            }
        }
        public int MostWins
        {
            get { return mostWins; }
            private set
            {
                mostWins = value;
                PlayerPrefs.SetInt("MostWins", value);
            }
        }
        public int MultiVictoryCount
        {
            get { return multiVictoryCount; }
            private set
            {
                multiVictoryCount = value;
                PlayerPrefs.SetInt("VictoryCount", value);
            }
        }
        public int SoloPlayCount
        {
            get { return totalPlayCount - multiPlayCount; }
        }
        public int MultiPlayCount
        {
            get { return multiPlayCount; }
            private set
            {
                multiPlayCount = value;
                PlayerPrefs.SetInt("MultiPlayCount", value);
            }
        }
        public int TotalPlayCount
        {
            get { return totalPlayCount; }
            private set
            {
                totalPlayCount = value;
                PlayerPrefs.SetInt("TotalPlayCount", value);
            }
        }
        public int TotalKillCount
        {
            get { return totalKillCount; }
            private set
            {
                totalKillCount = value;
                PlayerPrefs.SetInt("TotalKillCount", value);
            }
        }
        public int TotalDeathCount
        {
            get { return totalDeathCount; }
            private set
            {
                totalDeathCount = value;
                PlayerPrefs.SetInt("TotalDeathCount", value);
            }
        }
        public int MultiMostKillCount
        {
            get { return multiMostKillCount; }
            private set
            {
                multiMostKillCount = value;
                PlayerPrefs.SetInt("MultiMostKillCount", value);
            }
        }

        public PlayerGameData()
        {
            LoadPlayerGameData();
        }

        public void LoadPlayerGameData()
        {
            HighestScore = PlayerPrefs.GetInt("HighestScore", 0);
            HighestStage = PlayerPrefs.GetInt("HighestStage", -1);
            CurrentWins = PlayerPrefs.GetInt("CurrentWins", 0);
            MostWins = PlayerPrefs.GetInt("MostWins", 0);
            MultiVictoryCount = PlayerPrefs.GetInt("VictoryCount", 0);

            MultiPlayCount = PlayerPrefs.GetInt("MultiPlayCount", 0);
            TotalPlayCount = PlayerPrefs.GetInt("TotalPlayCount", 0);
            TotalKillCount = PlayerPrefs.GetInt("TotalKillCount", 0);
            TotalDeathCount = PlayerPrefs.GetInt("TotalDeathCount", 0);
            MultiMostKillCount = PlayerPrefs.GetInt("MultiMostKillCount", 0);
        }

        public void PlayerSoloPlayed()
        {
            TotalPlayCount += 1;
        }

        public bool PlayerScored(int score)
        {
            bool isNewHighScore = score > HighestScore;
            if (isNewHighScore)
                HighestScore = score;
            return isNewHighScore;
        }

        public void PlayerStagePlayed(int stage, bool isCleared)
        {
            PlayerSoloPlayed();
            if (isCleared && stage > HighestStage)
                HighestStage = stage;
        }

        public void PlayerAddKills(int killCount)
        {
            TotalKillCount += killCount;
            if (killCount > MultiMostKillCount)
                MultiMostKillCount = killCount;
        }

        public void PlayerAddDeaths(int deathCount)
        {
            TotalDeathCount += deathCount;
        }

        public void PlayerMultiWin()
        {
            TotalPlayCount += 1;
            MultiVictoryCount += 1;
            MultiPlayCount += 1;
            CurrentWins += 1;
            if (CurrentWins > MostWins)
                MostWins = CurrentWins;
        }

        public void PlayerMultiLose()
        {
            TotalPlayCount += 1;
            CurrentWins = 0;
            MultiPlayCount += 1;
        }

        public void ResetPlayerGameData()
        {
            PlayerPrefs.SetInt("HighestScore", 0);
            PlayerPrefs.SetInt("HighestStage", -1);
            PlayerPrefs.SetInt("CurrentWins", 0);
            PlayerPrefs.SetInt("MostWins", 0);
            PlayerPrefs.SetInt("VictoryCount", 0);
            PlayerPrefs.SetInt("MultiPlayCount", 0);
            PlayerPrefs.SetInt("TotalPlayCount", 0);
            PlayerPrefs.SetInt("TotalKillCount", 0);
            PlayerPrefs.SetInt("TotalDeathCount", 0);
            PlayerPrefs.SetInt("MultiMostKillCount", 0);
        }
    }
}
