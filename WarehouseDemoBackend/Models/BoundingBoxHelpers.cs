using System.Numerics;
using static WarehouseDemoBackend.Models.BotEnums;

namespace WarehouseDemoBackend.Models
{
    public class BoundingBoxHelpers
    {
        public BoundingBox StepBBFromDirection(BoundingBox BoxToMove, Direction direction, Vector2 currentSpeed)
        {
            Vector2 moveVector = new Vector2(0, 0);

            switch (direction)
            {
                case Direction.Up:
                    moveVector.Y = -(currentSpeed.Y);
                    break;
                case Direction.Down:
                    moveVector.Y = currentSpeed.Y;
                    break;
                case Direction.Left:
                    moveVector.X = -(currentSpeed.X);
                    break;
                case Direction.Right:
                    moveVector.X = currentSpeed.X;
                    break;
            }
            BoxToMove.UpdatePosition(moveVector);
            return BoxToMove;
        }

        public BoundingBox UnStepBBFromDirection(BoundingBox BoxToMove, Direction direction, Vector2 currentSpeed)
        {
            Vector2 moveVector = new Vector2(0, 0);

            switch (direction)
            {
                case Direction.Up:
                    moveVector.Y = currentSpeed.Y;
                    break;
                case Direction.Down:
                    moveVector.Y = -(currentSpeed.Y);
                    break;
                case Direction.Left:
                    moveVector.X = currentSpeed.X;
                    break;
                case Direction.Right:
                    moveVector.X = -(currentSpeed.X);
                    break;
            }
            BoxToMove.UpdatePosition(moveVector);
            return BoxToMove;
        }
    }
}
