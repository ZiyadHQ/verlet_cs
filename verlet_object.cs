using System.Numerics;
using Raylib_cs;

public class VerletObject
{

    public Vector2 position_current;
    public Vector2 position_old;
    public Vector2 force;
    public float radius = 5f;
    public float density = 1f;
    public float restitution = 1f;
    public Color color = Color.DarkGreen;

    public void applyForce(Vector2 acc)
    {
        force += acc;
    }

    public float getMass()
    {
        float area = radius * radius * MathF.PI;
        return area * density;
    }

    public void step(float dt, int steps = 1)
    {
        dt /= steps;
        for (int i = 0; i < steps; i++)
        {
            Vector2 velocity = position_current - position_old;
            Vector2 acceleration = force / getMass();

            position_old = position_current;

            position_current = position_current + velocity + force * dt * dt;

            force = Vector2.Zero;
        }
    }

    //the object cant go beyond the boundries of the imaginary circle of radius{float radius}
    public void pullBack(Vector2 center, float radius)
    {
        Vector2 direction = position_current - center;
        float distance = direction.Length();
        if (distance > radius - this.radius)
        {
            Vector2 newPos = Vector2.Normalize(direction) * (radius - this.radius) + center;
            float penetrationDepth = distance - (radius - this.radius);

            // Vector2 force = Vector2.Normalize(-direction) * penetrationDepth * restitution * getMass();
            // applyForce(force);
            position_current = newPos;
        }
    }

    public void draw()
    {
        Raylib.DrawCircleV(position_current, this.radius, color);
    }

}