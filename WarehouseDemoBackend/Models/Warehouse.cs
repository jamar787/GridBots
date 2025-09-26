using System;
using System.Numerics;

namespace WarehouseDemoBackend.Models
{
    public interface IWarehouse
    {
        public List<IBot> ActiveBots { get; set; }

        public List<IBoundingBox> BotLocations { get; set; }
        public List<IBoundingBox> GridLocations { get; set; }
        public BoundingBox Border {  get; set; }
        public Vector2 GridSquareLength { get; set; }
        public Vector2 GridLineLength { get; set; }
        public Vector2 BotLength { get; set; }
        public Vector2 NumGridlines { get; set; }
        public Vector2 NumGrids { get; set; }
        public float DefaultBotSpeed { get; set; }
        public double BotScalingFactor { get; set; }
        void UpdateNumGrids(Vector2 numGrids);
        void SetGridSizing();
        void Resize(Vector2 newBorderDimensions);
        String RandomColor();
        void StepForward();

        void AddNewBot(Vector2 TopLeftStartingPos, string defaultColor, double startingStepSpeed, BotEnums.OperationMode mode, bool useBrokenCycles, int brokenCycleLimit, int brokenCycleTarget, int breakChance, int directionChangeTargetVal, int directionChangeChance, int idleRollTargetVal, int idleChangeLimit);
        void RemoveBot(int id);

    }

    public class Warehouse : IWarehouse
    {
        public List<IBot> ActiveBots { get; set; }

        public List<IBoundingBox> BotLocations { get; set; }
        public List<IBoundingBox> GridLocations { get; set; }
        public BoundingBox Border { get; set; }
        public Vector2 GridSquareLength { get; set; }
        public Vector2 GridLineLength { get; set; }
        public Vector2 BotLength { get; set; }
        public Vector2 NumGridlines { get; set; }
        public Vector2 NumGrids { get; set; }
        public float DefaultBotSpeed { get; set; }
        public double BotScalingFactor { get; set; }

        public Warehouse(BoundingBox border, int numGridsX, int numGridsY, float botSpeed, double botScalingFactor)
        {
            this.ActiveBots = new List<IBot>();
            this.GridLocations = new List<IBoundingBox>();
            this.Border = border;
            this.BotScalingFactor = botScalingFactor;
            this.DefaultBotSpeed = botSpeed;
            this.NumGrids = new Vector2(numGridsX, numGridsY);
            this.NumGridlines = new Vector2(numGridsX + 1, numGridsY + 1);
            SetGridSizing();
        }

        public void UpdateNumGrids(Vector2 numGrids)
        {
            this.NumGrids = numGrids;
            this.NumGridlines = new Vector2(this.NumGrids.X + 1, this.NumGrids.Y + 1);
        }
        public void SetGridSizing()
        {
            Vector2 size = this.Border.GetSize();

            float X = (float)((size.X) / (this.NumGrids.X + (this.NumGridlines.X * this.BotScalingFactor)));
            float Y = (float)((size.Y) / (this.NumGrids.Y + (this.NumGridlines.Y * this.BotScalingFactor)));
            this.GridSquareLength = new Vector2(X, Y);
            // GridLineLen = Height - NumGrids*gridsquarelen / numgridlines
            float XLineLen = (size.X - this.NumGrids.X * this.GridSquareLength.X) / this.NumGridlines.X;
            float YLineLen = (size.Y - this.NumGrids.Y * this.GridSquareLength.Y) / this.NumGridlines.Y;
            this.GridLineLength = new Vector2(XLineLen, YLineLen);
            this.BotLength = new Vector2(XLineLen, YLineLen);
        }
        public void Resize(Vector2 newBorderDimensions)
        {
            this.Border.BottomRight = new Vector2(newBorderDimensions.X, newBorderDimensions.Y);
            SetGridSizing();
        }
        public String RandomColor()
        {
            return string.Format("#{0:X6}", Random.Shared.Next(0xFFFFFF));
        }
        public void StepForward()
        {
            foreach (Bot bot in ActiveBots) {
                bot.TakeStep(this.Border, this.GridLocations, this.BotLocations);
            }
        }

        public void AddNewBot(Vector2 TopLeftStartingPos, string defaultColor, double startingStepSpeed, BotEnums.OperationMode mode, bool useBrokenCycles, int brokenCycleLimit, int brokenCycleTarget, int breakChance, int directionChangeTargetVal, int directionChangeChance, int idleRollTargetVal, int idleChangeLimit)
        {
            int id = ActiveBots.Count;
            BotEnums.Status status;
            if (mode == BotEnums.OperationMode.Auto) {
                status = BotEnums.Status.STOPPED;
            }
            else
            {
                status = BotEnums.Status.MANUAL;
            }
            ActiveBots.Add(new Bot(id, BotEnums.Direction.Idle, defaultColor, startingStepSpeed, status, TopLeftStartingPos, this.BotLength.X, this.BotLength.Y, useBrokenCycles, brokenCycleLimit, brokenCycleTarget, breakChance, directionChangeTargetVal, directionChangeChance, idleRollTargetVal, idleChangeLimit));
        }
        public void RemoveBot(int id)
        {
            int removalIndex = -1;
            foreach (Bot bot in ActiveBots) {
                if (bot.Id == id) {
                    removalIndex = bot.Id; 
                    break;
                
                }
            }
            if (removalIndex >= 0) { 
            
                ActiveBots.RemoveAt(removalIndex);
            }
        }

    }
}
