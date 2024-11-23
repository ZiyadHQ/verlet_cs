
public class SpatialGrid
{

    public readonly int cellSize;
    public List<VerletObject>[,] Grid;

    public float minX, maxX;
    public float minY, maxY;

    public SpatialGrid(float minX, float maxX, float minY, float maxY, int cellSize = 10)
    {
        this.cellSize = cellSize;
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        Grid = new List<VerletObject>[cellSize, cellSize];
    }

    private void AddObject(VerletObject particle)
    {

        int i = (int)(particle.position_current.Y/cellSize);
        int j = (int)(particle.position_current.X/cellSize);

        Grid[i, j].Add(particle);
    }

    public void AssignObjects(List<VerletObject> list)
    {
        foreach(VerletObject particle in list){AddObject(particle);}
    }

    public List<VerletObject> getNeighborGroup(int i, int j)
    {



    }

}