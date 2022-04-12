namespace Capsule.Entity
{
    public enum TutorialType
    {
        MENU,
        MODE,
        OBSTACLE,
        INTERFACE,
        CONTROL,
    }

    public class TutorialData
    {
        public readonly string PauseGameDesc = "언제든지 게임을 멈출 수 있어요.";
        public readonly string TutorialDesc = "언제든지 튜토리얼을 다시 볼 수 있어요.";
        public readonly string TimeRemainDesc = "현재 남은 시간을 볼 수 있어요.";
        public readonly string StageTimeDesc = "시간이 다 지나면 클리어에 실패하니 주의하세요.";
        public readonly string SwiperDesc = "밀쳐내는 장애물이에요.";
        public readonly string SpikeRollerDesc = "가시가 날카로운 위험한 장애물이에요.";
        public readonly string ArcadeScoreDesc = "획득한 점수가 표시되어요.";
        public readonly string ArcadeTimeDesc = "시간이 다 지나면";
        public readonly string ArcadeWaveDesc = "현재 웨이브를 볼 수 있어요.";
        public readonly string ArcadeRemainEnemyDesc = "현재 남은 캡슐 수를 볼 수 있어요.";
        public readonly string ArcadeEnemyDesc = "스테이지 모드랑은 달라요.\n이제 적들도 공격하니 주의하세요!";
    }

    public class StageTutorialData : TutorialData
    {
        public readonly string RollTheBallDesc = "공을 굴려서 골인시키는 게임이에요.";
        public readonly string RollTheBallDescPC = "키보드 [W]로 전진 할 수 있어요.";
        public readonly string RollTheBallDescPC2 = "키보드 [S]로 후진 할 수 있어요.";
        public readonly string RollTheBallDescPC3 = "키보드 [A, D]로 좌우로 움직일 수 있어요.";
        public readonly string RollTheBallDescMobile = "[조이스틱]을 이용해 움직일 수 있어요.";

        public readonly string ActionButton1DescPC = "[마우스 왼쪽 클릭]을 누르면\n캡슐이 점프해요!";
        public readonly string ActionButton2DescPC = "[마우스 오른쪽 클릭]을 누르면\n캡슐이 앞으로 다이브해요!";
        public readonly string ActionButton1DescMobile = "[점프]버튼을 누르면 캡슐이 점프해요!";
        public readonly string ActionButton2DescMobile = "[다이브]버튼을 누르면 캡슐이 앞으로 다이브해요!";

        public readonly string SwiperDesc2 = "스테이지 밖으로 밀려나면 죽게된답니다.";
        public readonly string SpikeRollerDesc2 = "공이 닿으면 터져버리니 조심하세요!";
        public readonly string JumpDesc1 = "점프로 공을 옮겨 탈 수 있어요!";
        public readonly string JumpDesc2 = "길이 끊어져 있어도 점프로 넘어가자구요!";
        public readonly string DiveDesc1 = "다이브 해서 적 캡슐을 공격할 수 있어요!";
        public readonly string DiveDesc2 = "공격에 성공하면 공도 뺐어 탈 수 있답니다!";
        public readonly string DiveDesc3 = "적 캡슐도 항상 가만히 있는 건 아니라구요!";
        public readonly string DiveDesc4 = "신중하게 타이밍 맞춰서 다이브~!";
    }

    public class ArcadeTutorialData : TutorialData
    {

    }
}