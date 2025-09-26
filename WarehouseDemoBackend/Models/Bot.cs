//using WarehouseDemoBackend.Models.BotEnums;

using System;
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

        public Vector2 CurrentSpeed { get; set; }

        public Vector2 MaxSpeed { get; set; }

        void SetSpeed();

        bool CollidesWithObject(IBoundingBox BoxB);

        void Move(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations);

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

        public Vector2 CurrentSpeed { get; set; }
        public Vector2 MaxSpeed { get; set; }

        public BotHelpers.IBrokenConditions BotBrokenConditions { get; set; }

        public BotHelpers.IRandomHitCheck BrokenHitCheck { get; set; }

        public BotHelpers.IRandomHitCheck DirectionChangeCheck { get; set; }

        public BotHelpers.IRandomHitCheck IdleRollCheck { get; set; }

        public Bot(int id, BotEnums.Direction startingDirection, string defaultColor, double stepSpeed, BotEnums.Status initialStatus, Vector2 topLeftStart, Vector2 BotDimensions, bool useBrokenCycles, int brokenCycleLimit, int breakTargetVal, int breakChance, int directionChangeTargetVal, int directionChangeChance, int idleRollTargetVal, int idleChangeLimit)
        {
            this.Id = id;
            this.CurrentDirection = startingDirection;
            this.DefaultColor = defaultColor;
            this.CurrentColor = defaultColor;
            this.StepSpeed = stepSpeed;
            this.BotStatus = new BotHelpers.BotStatus();
            this.BotStatus.SetStatus(initialStatus, "Initialized");
            this.BoundingBox = new BoundingBox(topLeftStart, new Vector2(topLeftStart.X + BotDimensions.X, topLeftStart.Y+BotDimensions.Y));
            this.BotBrokenConditions = new BotHelpers.BrokenConditions(useBrokenCycles, brokenCycleLimit);
            this.BrokenHitCheck = new BotHelpers.RandomHitCheck(breakTargetVal, breakChance);
            this.DirectionChangeCheck = new BotHelpers.RandomHitCheck(directionChangeTargetVal, directionChangeChance);
            this.IdleRollCheck = new BotHelpers.RandomHitCheck(idleRollTargetVal, idleChangeLimit);
            SetSpeed();
        }

        public void SetSpeed()
        {
            this.MaxSpeed = this.BoundingBox.GetSize();
            this.CurrentSpeed = new Vector2((float)(this.MaxSpeed.X * this.StepSpeed), (float)(this.MaxSpeed.Y * this.StepSpeed));
            
        }
        public bool CollidesWithObject(IBoundingBox BoxB)
        {
            return BoundingBoxHelpers.GJKImplementation.DetectCollision(this.BoundingBox, BoxB);
        }

        public BotEnums.Direction IdleCheck(BotEnums.Direction testDirection)
        {
            if(testDirection == BotEnums.Direction.Idle && this.BotStatus.isBroken)
            {
                if (IdleRollCheck.IsRandomHit())
                {
                    while (testDirection == BotEnums.Direction.Idle) 
                    { 
                        testDirection = BotHelpers.GetRandomDirection();
                    }
                }
            }

            return testDirection;
        }

        public bool IsColliding(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations)
        {
            if (CollidesWithObject(border))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < GridLocations.Count; i++) {
                    if (CollidesWithObject(GridLocations[i]))
                    {
                        return true;
                    }
                }
                for (int j = 0; j < GridLocations.Count; j++)
                {
                    if (CollidesWithObject(GridLocations[j]))
                    {
                        return true;
                    }
                }
            }
                return false;
        }
        public void Move(IBoundingBox border, List<IBoundingBox> GridLocations, List<IBoundingBox> BotLocations)
        {
            BoundingBox newPosition = new BoundingBox(this.BoundingBox.TopLeft, this.BoundingBox.BottomRight);
            BotEnums.Direction testDirection = this.CurrentDirection;

            if (this.DirectionChangeCheck.IsRandomHit())
            {
                this.CurrentDirection = BotHelpers.GetRandomDirection();
                //Get Random Direction
            }

            testDirection = IdleCheck(testDirection);

            newPosition = BoundingBoxHelpers.StepBBFromDirection(newPosition, testDirection, this.CurrentSpeed);

            while(IsColliding(border, GridLocations, BotLocations))
            {
                newPosition = BoundingBoxHelpers.UnStepBBFromDirection(newPosition, testDirection, this.CurrentSpeed);
                testDirection = BotHelpers.GetRandomDirection();
                newPosition = BoundingBoxHelpers.StepBBFromDirection(newPosition, testDirection, this.CurrentSpeed);
            }

            this.CurrentDirection = testDirection;
            if (this.CurrentDirection == BotEnums.Direction.Idle) {
                this.BotStatus.isMoving = false;
            }
            else
            {
                this.BotStatus.isMoving = true;
            }

            this.BotStatus.UpdateStatus();
            this.BoundingBox = new BoundingBox(newPosition.TopLeft, newPosition.BottomRight);
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

            Move(border, GridLocations, BotLocations);

            if (BotBrokenConditions.BreakingChanceRoll(this.BrokenHitCheck))
            {
                this.BotStatus.SetStatus(BotEnums.Status.ERROR, "ERROR!");
                this.CurrentDirection = BotEnums.Direction.Idle;
                this.BotStatus.isMoving = false;
            }
        }
    }
}
