namespace DungeonGenerator;
public class Dungeon
{
    // Inner class to represent a cell : unit block of the dungeon
    public class Cell
    {
        public bool IsWall { get; set; }
        public Point Position { get; set; }
        public int X { get => Position.X; }
        public int Y { get => Position.Y; }
        public Cell(bool isWall, Point p)
        {
            IsWall = isWall;
            Position = p;
        }

        public override string ToString() => $"({X}, {Y})";
    }

    public Cell[,] Maze { get; set; }
    public int Size { get; set; }
    public List<Cell> DeadEnds = new List<Cell>();

    public Dungeon(int size)
    {
        if (size % 2 == 0) throw new ArgumentException("Size must be a odd number");
        this.Size = size;

        // Generate all cell
        Maze = new Cell[size, size];
        for (int i = 0; i < size; i ++)
        {
            for (int j = 0; j < size; j++)
            {
                bool isWall = i % 2 == 1 || j % 2 == 1;
                Point p = new(i, j);

                Maze[i, j] = new Cell(isWall, p);
            }
        }

        // Randomize DFS maze generation
        bool[,] visited = new bool[size, size];
        Stack<Cell> fringe = new Stack<Cell>();
        fringe.Push(Maze[0, 0]);

        while (fringe.Count > 0)
        {
            Cell c = fringe.Peek();
            visited[c.X, c.Y] = true;

            // Neighbor open cell is 2 unit away
            List<Cell> unvisitedNeigbors = Neightbor(c, 2) 
                .Where(nb => !visited[nb.X, nb.Y])
                .ToList();
            unvisitedNeigbors.Shuffle();

            if (unvisitedNeigbors.Count == 0)
            {
                int wallNeighborCount = Neightbor(c, 1).Where(nb => nb.IsWall).ToList().Count;
                if (wallNeighborCount == 3
                    || (wallNeighborCount == 2 && OnEdge(c))
                    || (wallNeighborCount == 1 && OnCorner(c)))
                {
                    DeadEnds.Add(c);
                }
                fringe.Pop();
                continue;
            }
            Cell next = unvisitedNeigbors[0];
            fringe.Push(next);
            OpenWall(c, next);
        }
    }

    private void OpenWall(Cell a, Cell b)
    {
        if (a.IsWall || b.IsWall) throw new ArgumentException("Both cell must not be wall.");

        int xDiff = Math.Abs(a.X - b.X);
        int yDiff = Math.Abs(a.Y - b.Y);
        if ((xDiff == 0 && yDiff != 2) || (xDiff == 2 && yDiff != 0))
        { 
            throw new ArgumentException($"Two cells is not neighbor: a({a.X}, {a.Y}), b({b.X}, {b.Y})");
        }

        int xHigher = a.X > b.X ? a.X : b.X;
        int yHigher = a.Y > b.Y ? a.Y : b.Y;

        if (a.X == b.X)
        {
            Maze[a.X, yHigher - 1].IsWall = false;
        }

        else // (a.Y == b.Y)
        {
            Maze[xHigher - 1, a.Y].IsWall = false;
        }
    }

    private List<Cell> Neightbor(Cell c, int offset)
    {
        List<Cell> neighbor = new List<Cell>();

        for (int i = -offset; i <= offset; i += offset)
        {
            for (int j = -offset; j <= offset; j += offset)
            {
                int nbX = c.X + i;
                int nbY = c.Y + j;
                int totalDiff = Math.Abs(i) + Math.Abs(j);
                if ((i == 0 && j == 0) || totalDiff != offset
                    || nbX < 0 || nbX > Size - 1 || nbY < 0 || nbY > Size - 1)
                {
                    continue;
                }
                neighbor.Add(Maze[nbX, nbY]);
            }
        }
        return neighbor;
    }

    // Modify fields DeadEnds in erosion
    public void EordeDeadEnds(int iteration)
    {
        for (int i = 0; i < iteration; i++)
        {
            List<Cell> nextDeadEnds = new List<Cell>();
            foreach (Cell deadEnd in DeadEnds)
            {
                List<Cell> prevOpenCells = Neightbor(deadEnd, 1)
                    .Where(nb => !nb.IsWall)
                    .ToList();
                if (prevOpenCells.Count != 1) continue;

                deadEnd.IsWall = true;
                nextDeadEnds.Add(prevOpenCells[0]);
            }
            DeadEnds = nextDeadEnds;
        }
    }

    // Determine whether a cell is on the edge i.e. At (top/ bot) row or (leftmost/ rightmost) col
    private bool OnEdge(Cell c)
    {
        return c.X == 0 || c.X == Size - 1 || c.Y == 0 || c.Y == Size - 1;
    }

    // Determine whether a cell is on the corner i.e. At up left, up right, lower left, lower right corner
    private bool OnCorner(Cell c)
    {
        return (c.X == 0 && c.Y == 0)
            || (c.X == 0 && c.Y == Size - 1)
            || (c.X == Size - 1 && c.Y == 0)
            || (c.X == Size - 1 && c.Y == Size - 1);
    }
}

public static class ListExtension
{
    // A list extension method to shuffle the list in place
    public static void Shuffle<T>(this IList<T> list)
    {
        Random rnd = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
