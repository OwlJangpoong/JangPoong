
    using System.Collections.Generic;

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
            hpSmall,
            hpLarge,
            mpSmall,
            mpLarge,
            invisibility,
            none,
        }

        public enum SaveKey
        {
            tutorialDone,
            playerHp,
            playerMana,
            levelToken,
            hpPotionSmallCnt,
            hpPotionLargeCnt,
            mpPotionSmallCnt,
            mpPotionLargeCnt,
            invisibilityPotionCnt,
            monsterPoint

        }
        // public static readonly Dictionary<string, string> FileNames = new Dictionary<string, string>
        // {
        //     { "playerInfo", "PlayerInfo.json" },
        //     { "playerStat", "PlayerStat.json" },
        //     { "inventory", "Inventory.json" },
        //     { "progress", "Progress.json" },
        //     { "statistics", "Statistics.json" },
        //     { "setting", "Settings.json" },
        // };
        //
        // public static readonly Dictionary<string, string> InitFileNames = new Dictionary<string, string>
        // {
        //     { "playerInfo", "PlayerInfo_init.json" },
        //     { "playerStat", "PlayerStat_init.json" },
        //     { "inventory", "Inventory_init.json" },
        //     { "progress", "Progress_init.json" },
        //     { "statistics", "Statistics_init.json" },
        //     { "setting", "Settings_init.json" },
        // };


        public enum FileName
        {
            PlayerInfo,
            PlayerStat,
            Inventory,
            Progress,
            Statistics,
            Setting
        }
    }

