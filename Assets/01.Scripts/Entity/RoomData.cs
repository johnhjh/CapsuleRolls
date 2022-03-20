namespace Capsule.Entity
{
    public class RoomData
    {
        private bool enableSearch;
        public bool EnableSearch
        {
            get { return enableSearch; }
            set { enableSearch = value; }
        }

        private bool usingPassword;
        public bool UsingPassword
        {
            get { return usingPassword; }
            set { usingPassword = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private int playerCount;
        public int PlayerCount
        {
            get { return playerCount; }
            set
            {
                if (value > 8) value = 8;
                else if (value < 1) value = 1;
                playerCount = value;
            }
        }

        private bool isTeamRandom;
        public bool IsTeamRandom
        {
            get { return isTeamRandom; }
            set { isTeamRandom = value; }
        }

        private bool isColorRandom;
        public bool IsColorRandom
        {
            get { return isColorRandom; }
            set { isColorRandom = value; }
        }
    }
}

