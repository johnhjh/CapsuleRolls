using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        LEVEL_MAX = 25600,
    }

    public static class LevelExpCalc
    {
        private static List<int> levelExpList;

        static LevelExpCalc()
        {
            levelExpList = new List<int>();
            levelExpList.Add((int)LevelExp.LEVEL_1);
            levelExpList.Add((int)LevelExp.LEVEL_2);
            levelExpList.Add((int)LevelExp.LEVEL_3);
            levelExpList.Add((int)LevelExp.LEVEL_4);
            levelExpList.Add((int)LevelExp.LEVEL_5);
            levelExpList.Add((int)LevelExp.LEVEL_6);
            levelExpList.Add((int)LevelExp.LEVEL_7);
            levelExpList.Add((int)LevelExp.LEVEL_8);
            levelExpList.Add((int)LevelExp.LEVEL_9);
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
            private set { playerNickName = value; }
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

        private void LevelUP()
        {
            Level += 1;
            PlayerPrefs.SetInt("PlayerLevel", Level);
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
            NickName = PlayerPrefs.GetString("PlayerNick", "캡슐파이터");
            Exp = PlayerPrefs.GetInt("PlayerExp", 0);
            Level = PlayerPrefs.GetInt("PlayerLevel", 1);
            Coin = PlayerPrefs.GetInt("PlayerCoins", 0);
            Rating = PlayerPrefs.GetInt("PlayerRating", 1200);
        }

        public void ResetPlayerData()
        {
            PlayerPrefs.SetString("PlayerNick", "캡슐파이터");
            PlayerPrefs.SetInt("PlayerExp", 0);
            PlayerPrefs.SetInt("PlayerLevel", 1);
            PlayerPrefs.SetInt("PlayerCoins", 50000);
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
                //Debug.Log("OpenData" + counter.ToString() + " : " + openData);
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
                //Debug.Log("BuyData" + counter.ToString() + " : " + buyData);
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
}
