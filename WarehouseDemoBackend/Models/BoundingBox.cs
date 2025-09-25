using System.Numerics;

namespace WarehouseDemoBackend.Models
{
    public interface IBoundingBox
    {
       Vector2 TopLeft { get; set; }
        Vector2 BottomRight { get; set; }
        List<Vector2> Coords { get; set; } 
       Vector2 GetSize();
        Vector2 GetCenter();

        void UpdatePosition(Vector2 MoveAmt);

        //void MultiplyByScalar(Vector2 vec, float s);
        //void DivideByScalar(Vector2 vec, float s);
        //void AddByScalar(Vector2 vec, float s);
        //void SubtractByScalar(Vector2 vec, float s);
    }
    public class BoundingBox : IBoundingBox
    {
        public Vector2 TopLeft { get; set; }
        public Vector2 BottomRight { get; set;}
        public List<Vector2> Coords { get; set; }

        public BoundingBox(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Coords = new List<Vector2>();
            Coords.Add(new Vector2(TopLeft.X, TopLeft.Y)); // TopLeft
            Coords.Add(new Vector2(BottomRight.X, TopLeft.Y)); //TopRight
            Coords.Add(new Vector2(BottomRight.X, BottomRight.Y)); //BottomRight
            Coords.Add(new Vector2(TopLeft.X, BottomRight.Y)); // BottomLeft
        }

        public Vector2 GetSize() { 
            return BottomRight - TopLeft;
        }
        public Vector2 GetCenter() {
            return Vector2.Subtract(BottomRight, divideByScalar(BottomRight, 2));
        }

        public void UpdatePosition(Vector2 MoveAmt)
        {
            this.TopLeft = new Vector2(this.TopLeft.X+MoveAmt.X, this.TopLeft.Y+MoveAmt.Y);
            Vector2 size = this.GetSize();
            this.BottomRight = new Vector2(this.TopLeft.X + size.X, this.TopLeft.Y + size.Y);
            Coords.Clear();
            Coords.Add(new Vector2(TopLeft.X, TopLeft.Y)); // TopLeft
            Coords.Add(new Vector2(BottomRight.X, TopLeft.Y)); //TopRight
            Coords.Add(new Vector2(BottomRight.X,BottomRight.Y)); //BottomRight
            Coords.Add(new Vector2(TopLeft.X, BottomRight.Y)); // BottomLeft
        }

        public static Vector2 multiplyByScalar(Vector2 vec, float s)
        {
            return new Vector2(vec.X * s, vec.Y * s);
        }

        public static Vector2 divideByScalar(Vector2 vec, float s)
        {
            return new Vector2(vec.X / s, vec.Y / s);
        }

        public static Vector2 addByScalar(Vector2 vec, float s)
        {
            return new Vector2(vec.X + s, vec.Y + s);
        }

        public static Vector2 subtractByScalar(Vector2 vec, float s)
        {
            return new Vector2(vec.X - s, vec.Y - s);
        }
    }
}
