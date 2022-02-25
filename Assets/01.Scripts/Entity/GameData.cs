using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Entity
{
    public enum GameMode
    {
        TUTORIAL = 0,
        PRACTICE,
        SOLO,
        MULTI,
        RANK,
        CUSTOM,
    }

    public enum GameKind
    {
        THROWING = 0,
        KILLINGROBOT,
        GOALIN,
    }

    public enum GameStage
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

        private GameStage stage;
        public GameStage Stage
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

