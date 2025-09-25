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

        bool TryToMove(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations);

        void TakeStep(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations);
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

        public BotHelpers.IBrokenConditions BotBrokenConditions { get; set; }

        public BotHelpers.IRandomHitCheck BrokenHitCheck { get; set; }

        public BotHelpers.IRandomHitCheck DirectionChangeCheck { get; set; }

        public BotHelpers.IRandomHitCheck IdleRollCheck { get; set; }

        public Bot(int id, BotEnums.Direction startingDirection, string defaultColor, double stepSpeed, BotEnums.Status initialStatus, Vector2 topLeftStart, float xSideLen, float ySideLen, bool useBrokenCycles, int brokenCycleLimit, int breakTargetVal, int breakChance, int directionChangeTargetVal, int directionChangeChance, int idleRollTargetVal, int idleChangeLimit)
        {
            this.Id = id;
            this.CurrentDirection = startingDirection;
            this.DefaultColor = defaultColor;
            this.CurrentColor = defaultColor;
            this.StepSpeed = stepSpeed;
            this.BotStatus = new BotHelpers.BotStatus();
            this.BotStatus.SetStatus(initialStatus, "Initialized");
            this.BoundingBox = new BoundingBox(topLeftStart, new Vector2(topLeftStart.X + xSideLen, topLeftStart.Y+ySideLen));
            this.BotBrokenConditions = new BotHelpers.BrokenConditions(useBrokenCycles, brokenCycleLimit);
            this.BrokenHitCheck = new BotHelpers.RandomHitCheck(breakTargetVal, breakChance);
            this.DirectionChangeCheck = new BotHelpers.RandomHitCheck(directionChangeTargetVal, directionChangeChance);
            this.IdleRollCheck = new BotHelpers.RandomHitCheck(idleRollTargetVal, idleChangeLimit);
        }

        public bool CollidesWithObject(IBoundingBox BoxB)
        {
            return BoundingBoxHelpers.GJKImplementation.DetectCollision(this.BoundingBox, BoxB);
        }

        public bool TryToMove(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations)
        {
            BoundingBox newPosition = new BoundingBox(this.BoundingBox.TopLeft, this.BoundingBox.BottomRight);
            BotEnums.Direction testDirection = this.CurrentDirection;

            if (this.DirectionChangeCheck.IsRandomHit())
            {
                //Get Random Direction
            }



            return false;
        }

        public void TakeStep(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations)
        {
            BotBrokenConditions.UpdateBrokenCycles(this.BotStatus);
            if (BotBrokenConditions.BrokenCycleReset) {
                this.BotStatus.isBroken = false;
                this.BotStatus.isMoving = false;
                this.BotStatus.SetStatus(BotEnums.Status.OK, "Error Resolved");
                this.BotBrokenConditions.BrokenCycleReset = false;
            }

            TryToMove(border, GridLocations, BotLocations);

            if (BotBrokenConditions.BreakingChanceRoll(this.BrokenHitCheck))
            {
                this.BotStatus.SetStatus(BotEnums.Status.ERROR, "ERROR!");
                this.CurrentDirection = BotEnums.Direction.Idle;
                this.BotStatus.isMoving = false;
            }
        }
    }
}
