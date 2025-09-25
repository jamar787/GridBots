using Microsoft.AspNetCore.Http.Features;

namespace WarehouseDemoBackend.Models
{
    public class BotHelpers
    {
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
    }
}
