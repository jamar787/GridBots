//using WarehouseDemoBackend.Models.BotEnums;

using System.Numerics;

namespace WarehouseDemoBackend.Models
{
    public interface IBot
    {
        public int Id { get; set; }
        public BotEnums.Direction CurrentDirection { get; set; }

        public BotEnums.OperationMode CurrentOperationMode { get; set; }

        public string CurrentColor { get; set; }

        public string DefaultColor { get; set; }

        public double StepSpeed { get; set; }

        public BotHelpers.IBotStatus BotStatus { get; set; }

        public IBoundingBox BoundingBox { get; set; }

        bool CollidesWithObject(IBoundingBox BoxB);

        bool TryToMove();

        void TakeStep();
    }

    public class Bot : IBot
    {

        public int Id { get; set; }
        public BotEnums.Direction CurrentDirection { get; set; }

        public BotEnums.OperationMode CurrentOperationMode { get; set; }

        public string CurrentColor { get; set; }

        public string DefaultColor { get; set; }

        public double StepSpeed { get; set; }

        public BotHelpers.IBotStatus BotStatus { get; set; }

        public IBoundingBox BoundingBox { get; set; }

        public Bot(int id, BotEnums.Direction startingDirection, string defaultColor, double stepSpeed, BotEnums.Status initialStatus, Vector2 topLeftStart, float xSideLen, float ySideLen)
        {
            this.Id = id;
            this.CurrentDirection = startingDirection;
            this.DefaultColor = defaultColor;
            this.CurrentColor = defaultColor;
            this.StepSpeed = stepSpeed;
            this.BotStatus = new BotHelpers.BotStatus();
            this.BotStatus.SetStatus(initialStatus, "Initialized");
            this.BoundingBox = new BoundingBox(topLeftStart, new Vector2(topLeftStart.X + xSideLen, topLeftStart.Y+ySideLen));
        }

        public bool CollidesWithObject(IBoundingBox BoxB)
        {
            return false;
        }

        public bool TryToMove()
        {
            return false;
        }

        public void TakeStep()
        {

        }


    }
}
