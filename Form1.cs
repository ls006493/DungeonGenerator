
namespace DungeonGenerator;

public partial class Form1 : Form
{
    static readonly (int ds, int cw)[] sizeOptions = { (401, 2), (201, 4), (101, 8), (51, 16), (25, 32) };
    static int sizeChoice = 2;
    static int dungeonSize = sizeOptions[sizeChoice].ds;
    static int cellWidth = sizeOptions[sizeChoice].cw;

    Dungeon dungeon = new Dungeon(dungeonSize);
    bool erodeMode = false;

    public Form1()
    {
        InitializeComponent();
    }

    private void canvas_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        this.ClientSize = new Size(dungeonSize * cellWidth, dungeonSize * cellWidth);

        Brush blackBrush = new SolidBrush(Color.Black);
        Brush whiteBrush = new SolidBrush(Color.White);
        Brush greenBrush = new SolidBrush(Color.Green);

        DrawDungeon(g, blackBrush, whiteBrush);

        if (erodeMode) HighlightCells(g, greenBrush, dungeon.DeadEnds);
    }

    private void DrawDungeon(Graphics g, Brush wallBrush, Brush openCellBrush)
    {
        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                int pixelX = i * cellWidth;
                int pixelY = j * cellWidth;

                Brush choosenBrush = dungeon.Maze[i, j].IsWall ? wallBrush : openCellBrush;

                g.FillRectangle(choosenBrush, pixelX, pixelY, cellWidth, cellWidth);
            }
        }
    }

    private void HighlightCells(Graphics g, Brush highlightBrush, List<Dungeon.Cell> cells)
    {
        foreach (Dungeon.Cell cell in cells)
        {
            int pixelX = cell.X * cellWidth;
            int pixelY = cell.Y * cellWidth;
            g.FillRectangle(highlightBrush, pixelX, pixelY, cellWidth, cellWidth);
        }
    }

    private void canvas_Click(object sender, EventArgs e)
    {
        MouseEventArgs me = (MouseEventArgs) e;
        switch (me.Button)
        {
            case MouseButtons.Left:
                erodeMode = false;
                dungeon = new Dungeon(dungeonSize);
                break;

            case MouseButtons.Right:
                erodeMode = true;
                dungeon.EordeDeadEnds(1);
                break;
        }
        this.Refresh();

    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
                if (sizeChoice < sizeOptions.Length - 1)
                {
                    sizeChoice++;
                    dungeonSize = sizeOptions[sizeChoice].ds;
                    cellWidth = sizeOptions[sizeChoice].cw;
                }
                else
                {
                    return;
                }
                break;

            case Keys.Down:
                if (sizeChoice > 0)
                {
                    sizeChoice--;
                    dungeonSize = sizeOptions[sizeChoice].ds;
                    cellWidth = sizeOptions[sizeChoice].cw;
                }
                else
                {
                    return;
                }
                break;

            default:
                return;
        }
        // Console.WriteLine($"dungeonSize {dungeonSize}, cellWidth {cellWidth}");
        dungeon = new Dungeon(dungeonSize);
        this.Refresh();
    }
}
