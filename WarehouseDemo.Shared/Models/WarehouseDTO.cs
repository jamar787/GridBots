using System.Numerics;

namespace WarehouseDemo.Shared.Models
{
    public class AddBotRequest
    {
        public float BotXLen { get; set; }
        public float BotYLen { get; set; }
        public string Color { get; set; } = "#0F0F0F";
        public double StepSpeed { get; set; }
        public bool IsAutomatic { get; set; }
        public bool UseBrokenCycles { get; set; }
        public int BrokenCycleLimit { get; set; }
        public int BrokenCycleTarget { get; set; }
        public int BreakChance { get; set; }
        public int DirectionChangeTargetVal { get; set; }
        public int DirectionChangeChance { get; set; }
        public int IdleRollTargetVal { get; set; }
        public int IdleChangeLimit { get; set; }
    }

    public class RemoveBotRequest
    {
        public int BotId { get; set; }
    }

    public class UpdateWarehouseRequest
    {
        public float BorderWidth { get; set; }
        public float BorderHeight { get; set; }
        public int NumGridsX { get; set; }
        public int NumGridsY { get; set; }
        public float DefaultBotSpeed { get; set; }
        public double BotScalingFactor { get; set; }
    }

}

