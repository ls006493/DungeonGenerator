namespace DungeonGenerator;

public partial class Form1 : Form
{
    readonly static int dungeonSize = 101;
    readonly static int cellWidth = 5;
    readonly Dungeon dungeon = new Dungeon(dungeonSize);

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

        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                int pixelX = i * cellWidth;
                int pixelY = j * cellWidth;

                Brush choosenBrush = dungeon.Maze[i, j].IsWall ? blackBrush : whiteBrush;

                g.FillRectangle(choosenBrush, pixelX, pixelY, cellWidth, cellWidth);
            }
        }
    }
}
