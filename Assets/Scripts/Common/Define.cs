
    using System.Collections.Generic;
    using System.Linq;

    public class Define
    {
        public enum WorldObject
        {
            Unknown,
            Player,
            Monster,

        }

        public enum Layer
        {
            Default = 0,
            PlayerDamaged = 3,
            Ground = 6,
            Level1 = 7,
            LevelN = 8,
            Obstacle = 9,

            Player = 15,
            Monster = 16,
            MonsterDie = 19,

        }

        public enum State
        {
            Die,
            Moving,
            Jumping,
            Idle,
            Attack,
            Target,
        }

        public enum UIEvent
        {
            Click,
            Drag,
        }

        public enum MouseEvent
        {
            Press,
            PointerDown,
            PointerUp,
            Click,
        }

        public enum Sound
        {
            Bgm,
            Sfx,
            MaxCount,
        }

        public enum Item
        {
            hpPotionSmall,
            hpPotionLarge,
            mpPotionSmall,
            mpPotionLarge,
            invisibilityPotion,
            levelUpToken,
            length,
        }
        
        //키보드 키코드 (데이터 저장할 때 Dictionary Key로 사용되므로 변경 시 주의!)
        public enum ControlKey
        {
            leftKey,
            rightKey,
            jumpKey,
            slideKey,
            runKey,
            attackKey,
            ultiKey,
        }


        #region Saving Data
        public enum SaveKey
        {
            tutorialDone,
            PlayerData,
            ProgressData,
            StatisticData,
            SettingData,
            InventoryData,

        }
        
        
 
        
        
        //file name 효율적으로 관리하기 위해 Dictionary 자료구조 사용 (하드 코딩 대신)
        //키 : 파일 용도 등 파일 이름 구분자
        //값 : 파일 이름(json 파일 이름)
        /// <summary>
        /// Key : PlayerData, ProgressData, StatisticData, SettingData
        /// </summary>
        public static readonly Dictionary<string, string> FileNames = new Dictionary<string, string>
        {
            { "PlayerData", "PlayerData.json" },
            { "ProgressData", "Progress.json" },
            { "StatisticData", "Statistic.json" },
            { "SettingData", "Setting.json" },
            {"InventoryData", "Inventory.json"}

        };

        public static readonly Dictionary<string, string> SceneNames = new Dictionary<string, string>
        {
            { "인트로", "0 Intro" },
            { "0-1 튜토리얼", "0-1 tutorial" },
            { "0-2 튜토리얼", "0-2 tutorial" },
            { "0-3 튜토리얼", "0-3 tutorial" },
            { "1-1 여행의 시작", "1-1 tutorial" },
            { "1-2 숲 속", "1-2 forest" },
            { "1-3 고블린 퇴치", "1-3 goblin fight" },
            { "1-4 마왕의 숲", "1-4 Dialogue with Slivan" },
            { "2-1 마왕의 숲", "2-1 to demon castle" },
            {"2-2-1 실비아 구출","2-2-2 saving silvia"},
            { "2-2-2 마왕성 앞 묘지", "2-2 to demon castle" },
            { "2-3-1 마왕성", "2-3 inside the castle" },
            {"2-3-2 마왕과의 전투", "Boss fight"},
            { "2-3-2 국왕 구출", "2-3-2 inside the castle" },
            {"엔딩","Ending cut scene"}
        };
        
        public static readonly Dictionary<string, string> ReverseSceneNames = SceneNames.ToDictionary(x => x.Value, x => x.Key);

        

        #endregion
        

        #region JangPoong

        public enum JangPoongLevel
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
            Level8,
            Level9,
            Level10,
            Length
        }

        public static readonly float[] JangPoongDamageList =
        {
            0.5f, 0.9f, 1.4f, 1.9f, 2.6f, 3.5f, 4.1f, 4.9f, 5.6f, 6.5f
        };

        #endregion



    }
   

