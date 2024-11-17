
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

public class Constraint
{
    public VerletObject a;
    public VerletObject b;
    public float length;

    public Constraint(VerletObject a, VerletObject b, float length)
    {
        this.a = a;
        this.b = b;
        this.length = length;
        Debug.Assert(length > a.radius + b.radius);
    }

    public Vector2 getCenter()
    {
        return new Vector2((a.position_current.X + b.position_current.X)/2, (a.position_current.Y + b.position_current.Y)/2);
    }

    public void constrain()
    {
        Vector2 direction = a.position_current - b.position_current;
        float distance = direction.Length();
        if(distance != length)
        {
            direction = Vector2.Normalize(direction);
            Vector2 ceneter = getCenter();
            Vector2 newA = ceneter + direction * length/2;
            Vector2 newB = ceneter - direction * length/2;

            a.position_current = newA;
            b.position_current = newB;
        }
    }

    public void draw()
    {
        Raylib.DrawLineEx(a.position_current, b.position_current, 2f, Color.DarkPurple);
    }
}