namespace NormalDistribution
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool InBounds(Point bound1, Point bound2)
        {
            float x1 = bound1.X;
            float x2 = bound2.X;
            float y1 = bound1.Y;
            float y2 = bound2.Y;

            if (bound1.X > bound2.X)
            {
                x1 = bound2.X;
                x2 = bound1.X;
            }
            if (bound1.Y > bound2.Y)
            {
                y1 = bound2.Y;
                y2 = bound1.Y;
            }

            return (X <= x2) && (X >= x1) && (Y <= y2) && (Y >= y1);
        }

        public static Point FindIntersection(Point s1, Point e1,
           Point s2, Point e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;
            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;
            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                return new Point(float.NaN, float.NaN);
            }

            var result = new Point(
                (b2 * c1 - b1 * c2) / delta,
                (a1 * c2 - a2 * c1) / delta);

            if (result.InBounds(s1, e1) && result.InBounds(s2, e2))
            {
                return result;
            }

            return new Point(float.NaN, float.NaN);
        }

        public static (float X, float Y) FindIntersection((float X, float Y) s1, (float X, float Y) e1,
           (float X, float Y) s2, (float X, float Y) e2)
        {
            Point result = FindIntersection(new Point(s1.X, s1.Y), new Point(e1.X, e1.Y), 
                new Point(s2.X, s2.Y), new Point(e2.X, e2.Y));
            return (result.X, result.Y);
        }

        public override string ToString()
        {
            return base.ToString() + $" ({X}; {Y}) ";
        }
    }
}