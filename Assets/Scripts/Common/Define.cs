
    public class Define
    {
        public enum WorldObject
        {
            Unknown,
            Player,
            Slime,
            Goblin,
        }

        public enum Layer
        {
            Default = 0,
            PlayerDamaged = 3,
            Ground = 6,
            Level1=7,
            LevelN=8,
            Obstacle = 9,
            
            Player = 15,
            Monster = 16,
            
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

        public float tileWidth = 0f;
    }
