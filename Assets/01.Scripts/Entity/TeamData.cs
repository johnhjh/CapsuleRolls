using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Entity
{
    public class TeamData
    {
        private Color teamColor;
        public Color TeamColor
        {
            get { return teamColor; }
            set { teamColor = value; }
        }

        private int teamScore;
        public int TeamScore
        {
            get { return teamScore; }
            set { teamScore = value; }
        }

        private int winRounds;
        public int WinRounds
        {
            get { return winRounds; }
            set { winRounds = value; }
        }
    }
}

