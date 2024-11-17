
using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;

public static class MathEX
{

    private static float Cross(Vector2 O, Vector2 a, Vector2 b)
    {
        return (a.X - O.X) * (b.Y - O.Y) - (a.Y - O.Y) * (b.X - O.X);
    }

    public static List<Vector2> getConvexHull(List<Vector2> points)
    {

        if(points.Count < 3)
        {
            Console.WriteLine("ERROR! list muse contain at least 3 points!");
            return points;
        }

        int n = points.Count, k = 0;

        List<Vector2> H = new List<Vector2>(2 * n);

        points.Sort((a, b) => (a.X != b.X)? a.X.CompareTo(b.X): a.Y.CompareTo(b.Y));

        for(int i=0; i<n; i++)
        {
            while(k >= 2 && Cross(H[k-2], H[k-1], points[i]) <= 0)
            {
                k--;
            }
            k++;
            H.Add(points[i]);
        }

        for(int i = n - 2, t = k + 1; i >= 0; i--)
        {
            while(k >= t && Cross(H[k - 2], H[k - 1], points[i]) <= 0)
            {
                k--;
                k++;
                H.Add(points[i]);
            }
        }

        if(k > 1)
        {
            H = H.GetRange(0, k-1);
        }

        return H;
    }

    public static void drawListOfPoints(List<Vector2> points)
    {

        for(int i=0; i<points.Count - 1; i++)
        {

            Raylib.DrawLineEx(points[i], points[i + 1], 2f, Color.SkyBlue);

        }

    }

}