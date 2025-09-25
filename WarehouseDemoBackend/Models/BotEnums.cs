

namespace WarehouseDemoBackend.Models
{
    public class BotEnums
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Idle
        }

        public enum Status
        {
            OK,
            ERROR,
            STOPPED,
            MANUAL
        }
        public enum OperationMode
        {
            Auto,
            Manual
        }
    }
}
