using Microsoft.AspNetCore.Http.Features;
using System;
using static WarehouseDemoBackend.Models.BotEnums;

namespace WarehouseDemoBackend.Models
{
    public class BotHelpers
    {


        public static BotEnums.Direction GetRandomDirection()
        {
                Array values = Enum.GetValues(typeof(Direction));
                return (Direction)values.GetValue(Random.Shared.Next(values.Length));   
        }
        public interface IBotStatus
        {
            BotEnums.Status Status { get; set; }
            static string STOPPEDSTATUSCOLOR { get; set; }
            static string ERRORSTATUSCOLOR { get; set; }
            static string OKSTATUSCOLOR { get; set; }
            static string MANUALSTATUSCOLOR { get; set; }
            bool isMoving { get; set; }
            bool isBroken { get; set; }
            string DisplayMessage { get; set; }
            string CurrentStatusColor { get; set; }

            void SetStatus(BotEnums.Status status, string message);

            void UpdateStatus();
        }

        public class BotStatus : IBotStatus
        {
            public BotEnums.Status Status { get; set; }
            public static string STOPPEDSTATUSCOLOR { get; set; } = "#e60000";
            public static string ERRORSTATUSCOLOR { get; set; } = "#f5dd42";
            public static string OKSTATUSCOLOR { get; set; } = "#24a800";
            public static string MANUALSTATUSCOLOR { get; set; } = "#660099";

            public bool isMoving { get; set; }
            public bool isBroken { get; set; }
            public string DisplayMessage { get; set; } = "";

            public string CurrentStatusColor { get; set; } = STOPPEDSTATUSCOLOR;
            
            public void SetStatus(BotEnums.Status status, string message)
            {
                this.Status = status;
                this.DisplayMessage = message;
                switch (status)
                {
                    case BotEnums.Status.OK:
                        this.CurrentStatusColor = OKSTATUSCOLOR;
                        this.isMoving = true;
                        this.isBroken = false;
       
                        break;
                    case BotEnums.Status.ERROR:
                        this.CurrentStatusColor = ERRORSTATUSCOLOR;
                        this.isMoving = false;
                        this.isBroken = true;
                       
                        break;
                    case BotEnums.Status.STOPPED:
                        this.CurrentStatusColor = STOPPEDSTATUSCOLOR;
                        this.isMoving = false;
                        this.isBroken = false;
                        
                        break;
                    case BotEnums.Status.MANUAL:
                        this.CurrentStatusColor = MANUALSTATUSCOLOR;
                        this.isMoving = true;
                        this.isBroken = false;
                        
                        break;
                }

            }

            public void UpdateStatus()
            {
                if (this.isBroken)
                    SetStatus(Status.ERROR, "ERROR");
                else if (!this.isMoving)
                    SetStatus(Status.STOPPED, "Stopped");
                else if (this.Status == Status.MANUAL)
                    SetStatus(Status.MANUAL, "Manual");
                else
                    SetStatus(Status.OK, "Ok");
            }

        }

        public interface IRandomHitCheck
        {
            public int TargetHitValue { get; set; }
            public int RandomChanceRange { get; set; }

            bool IsRandomHit();

        }
        public class RandomHitCheck : IRandomHitCheck
        {
            public int TargetHitValue { get; set; }
            public int RandomChanceRange { get; set; }

            public RandomHitCheck(int targetHitValue, int RandomChanceRange) 
            {
                this.TargetHitValue = targetHitValue;
                this.RandomChanceRange = RandomChanceRange;
            }

            public bool IsRandomHit()
            {
                return Random.Shared.Next(0, RandomChanceRange) == TargetHitValue;
            }

        }

        public interface IBrokenConditions
        {
            bool UseBrokenCycles { get; set; }
            int NumBrokenCycles { get; set; }
            int BrokenCycleLimit { get; set; }
            bool BrokenCycleReset { get; set; }
            
            void UpdateBrokenCycles(IBotStatus status);
            bool BreakingChanceRoll(IRandomHitCheck HitCheck);
        }

        public class BrokenConditions : IBrokenConditions
        {
            public bool UseBrokenCycles { get; set; }
            public int NumBrokenCycles { get; set; }
            public int BrokenCycleLimit { get; set; }
            public bool BrokenCycleReset { get; set; }

            public BrokenConditions(bool useBrokenCycles, int brokenCycleLimit)
            {
                UseBrokenCycles = useBrokenCycles;
                BrokenCycleLimit = brokenCycleLimit;
                BrokenCycleReset = false;
                NumBrokenCycles = 0;
            }

            public void UpdateBrokenCycles(IBotStatus status)
            {
                if (this.UseBrokenCycles)
                {
                    if (status.isBroken)
                    {
                        if (this.NumBrokenCycles < this.BrokenCycleLimit)
                        {
                            this.NumBrokenCycles++;
                            this.BrokenCycleReset = false;
                            return;
                        }
                        else
                        {
                            this.NumBrokenCycles = 0;
                            this.BrokenCycleReset = true;
                            status.isBroken = false;
                            return;
                        }
                    }
                }
            }
            public bool BreakingChanceRoll(IRandomHitCheck HitCheck)
            {
                if (this.UseBrokenCycles)
                {
                    return HitCheck.IsRandomHit();
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
