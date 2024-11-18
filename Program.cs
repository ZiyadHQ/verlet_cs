using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

internal class Program
{
    public static Random random = new();
    public static void shootProjectile(List<VerletObject> list, Vector2 position, Vector2 direction)
    {
        Color[] colors =
        [
            Color.DarkGreen,
            Color.Blue,
            Color.Purple,
            Color.Yellow
        ];
        VerletObject obj = new()
        {
            position_current = position,
            position_old = position,
            color = colors[random.Next(colors.Length)]
        };
        // obj.radius += random.NextSingle() * 5f;

        Raylib.DrawCircleLinesV(position, 10f, Color.Brown);

        obj.applyForce(direction * 10000f);

        list.Add(obj);
    }

    public static void resolveCollisions(List<VerletObject> list, int steps = 1)
    {

        for (int step = 0; step < steps; step++)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    VerletObject obj1 = list[i];
                    VerletObject obj2 = list[j];
                    Vector2 direction = obj2.position_current - obj1.position_current;
                    float distance = direction.Length();
                    if (distance < obj1.radius + obj2.radius)
                    {
                        direction = Vector2.Normalize(direction);
                        float displacement = obj1.radius + obj2.radius - distance;
                        displacement /= 2;
                        obj1.position_current -= direction * displacement;
                        obj2.position_current += direction * displacement;
                    }
                }
            }
        }

    }

    public static Vector2 GenerateRandomNormal()
    {
        // Generate a random angle between 0 and 2π (360 degrees)
        float angle = (float)(random.NextDouble() * 2 * Math.PI);

        // Convert the angle to a unit vector (cosine and sine)
        Vector2 normal = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        return Vector2.Normalize(normal);
    }

    private static void Main(string[] args)
    {

        Vector2 center = new(640, 300);

        Raylib.InitWindow(1280, 600, "Window");
        Raylib.SetTargetFPS(0);
        Camera2D camera = new(); ;

        List<VerletObject> list = [];
        List<Constraint> listOfConstraints = [];
        list.Add(new VerletObject()
        {
            position_current = new(640, 300),
            position_old = new(640, 300),
            radius = 5f
        });

        list.Add(new VerletObject()
        {
            position_current = new(620, 300),
            position_old = new(620, 300),
            radius = 5f
        });

        listOfConstraints.Add
        (
            new Constraint(list[0], list[1], 100f)
        );

        Stopwatch timer = new();

        while (!Raylib.WindowShouldClose())
        {
            timer.Start();
            float dt = Raylib.GetFrameTime();
            Raylib.BeginMode2D(camera);
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            Raylib.DrawCircleV(center, 350f, Color.Black);

            if (Raylib.IsKeyDown(KeyboardKey.Right))
            {
                foreach (VerletObject obj in list) { obj.applyForce(new(100f, 0f)); }
            }
            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                shootProjectile(list, center, GenerateRandomNormal());
            }
            if (Raylib.IsKeyReleased(KeyboardKey.Q))
            {
                shootProjectile(list, center, GenerateRandomNormal());
            }
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                foreach (VerletObject obj in list)
                {
                    Vector2 direction = mouse - obj.position_current;
                    direction = Vector2.Normalize(direction);
                    obj.applyForce(direction * 1000f);
                }
            }
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                VerletObject obj = list[0];
                Vector2 direction = mouse - obj.position_current;
                direction = Vector2.Normalize(direction);
                obj.applyForce(direction * 1000f);
            }
            if (Raylib.IsKeyReleased(KeyboardKey.LeftControl))
            {
                listOfConstraints.Clear();
                int polygonCount = 2;
                for (int i = 0; i < list.Count - (polygonCount - 1); i += polygonCount + 1)
                {
                    for (int j = 0; j < polygonCount; j++)
                    {
                        listOfConstraints.Add(new Constraint(list[i + j], list[i + j + 1], list[i + j].radius + list[i + j + 1].radius + 4f));
                    }
                    listOfConstraints.Add(new Constraint(list[i], list[i + polygonCount], list[i].radius + list[i + polygonCount].radius + 4f));
                }
            }
            if (Raylib.IsKeyReleased(KeyboardKey.Up))
            {
                camera.Zoom--;
            }
            if (Raylib.IsKeyReleased(KeyboardKey.Down))
            {
                camera.Zoom++;
            }

            int subSteps = 4;
            for (int i = 0; i < subSteps; i++)
            {
                float sub_dt = dt/subSteps;
                resolveCollisions(list);

                foreach (Constraint constraint in listOfConstraints)
                {
                    constraint.constrain();
                    constraint.draw();
                }

                foreach (VerletObject obj in list)
                {
                    // obj.applyForce(new(0f, 250f));
                    // obj.pullBack(center, 350f);
                    obj.step(sub_dt, 1);
                }
            }
            foreach(VerletObject obj in list){obj.draw();}

            Raylib.DrawCircleV(Raylib.GetMousePosition(), 10f, Color.Red);

            Raylib.DrawText($"frame time: {timer.Elapsed}\nObject Count: {list.Count}\ncamera: {camera.Zoom}", 0, 0, 24, Color.DarkGreen);
            timer.Reset();
            Raylib.EndDrawing();
            Raylib.EndMode2D();
        }
    }
}