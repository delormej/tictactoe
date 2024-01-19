
public class Board : IEquatable<Board>
{
    const int Boxes = 9; 

    // All possible boards

    // All possible win vectors
    static readonly int[,] WinVectors = new [,]
    { 
        // horizontal
        { 0, 1, 2 }, 
        { 3, 4, 5 }, 
        { 6, 7, 8 },
        // vertical
        { 0, 3, 6 },
        { 1, 4, 7 },
        { 2, 5, 8 },
        // diagonal
        { 0, 4, 8 },
        { 2, 4, 6 }
    };

    public static Board StartingBoard = 
        new(Enumerable.Repeat(Player.Empty, Boxes).ToArray());

    Board(Player[] grid)
    {
        _grid = grid;
        _random = new();
        _isWin = CalculateIsWin();
        _isDraw = CalculateIsDraw();
        // _movesForX = GetAvailableMoves(Player.X).ToList();
        // _movesForO = GetAvailableMoves(Player.O).ToList();
    }

    Board? _move;
    Player? _player;
    readonly bool _isWin, _isDraw;
    readonly Random _random;
    readonly Player[] _grid;
    List<Board>? _movesForX;
    List<Board>? _movesForO;

    /// <summary>
    /// Computer plays a move for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Board Move(Player player)
    {
        if (IsGameOver)
            throw new Exception("Game is over");

        _player = player;

        LoadMoves();
        // get a random, available move
        var moves = _player == Player.X ? _movesForX : _movesForO;

        if (moves == null || moves.Count == 0)
        {
            // Ran out of moves, reset.
            moves = GetAvailableMoves(player).ToList();
        }

        _move = moves[_random.Next(0, moves.Count)];
        
        // remove from available moves
        moves.Remove(_move);

        return _move;
    }

    public Board Move(Player player, int position)
    {
        if (position < 0 || position >= Boxes)
            throw new Exception("Invalid position");

        if (_grid[position] != Player.Empty)
            throw new Exception("Position already taken");

        Player[] newGrid = [.._grid];
        newGrid[position] = player;

        _player = player;
        _move = GetBoard(newGrid);

        return _move;
    }

    public void LoadMoves()
    {
        if (_movesForX == null)
            _movesForX = GetAvailableMoves(Player.X).ToList();
        if (_movesForO == null)            
            _movesForO = GetAvailableMoves(Player.O).ToList();
    }

    public bool IsGameOver => IsWin || IsDraw;
    
    public bool IsWin => _isWin;

    public bool IsDraw => _isDraw;

    /// <summary>
    /// Reinforces wins & draws, and penalizes losses.
    /// </summary>
    /// <exception cref="Exception">Move not played yet</exception>
    public void Learn()
    {
        if (_move == null)
            throw new Exception("Move not played");

        var moves = _player == Player.X ? _movesForX : _movesForO;

        // Add 2 for a Win, 1 for a Draw, 0 for a Loss
        int count = IsWin ? 2 : (IsDraw ? 1 : 0);

        if (moves == null)
            moves = new();

        for (int i = 0; i < count; i++)
        {
            moves.Add(_move);
        }

        // Clear the move for the next learning opportunity.
        _move = null;
    }

    /// <summary>
    /// Checks each potential win vector; horizontal, vertical, diagnol.
    /// </summary>
    /// <returns></returns>
    private bool CalculateIsWin()
    {
        for (int i = 0; i < WinVectors.GetLength(0); i++)
        {
            var sum = Math.Abs( 
                (int)_grid[WinVectors[i, 0]] + 
                (int)_grid[WinVectors[i, 1]] + 
                (int)_grid[WinVectors[i, 2]]
            );

            if (sum == 3) return true;
        }
        
        return false;
    }

    /// <summary>
    /// If all cells are filled, it's a draw.
    /// </summary>
    /// <returns></returns>
    private bool CalculateIsDraw()
    {
        var sum = _grid.Cast<int>().Select(i => Math.Abs(i)).Sum();
        return sum == Boxes;
    }

    private IEnumerable<Board> GetAvailableMoves(Player player)
    {
        for (int i = 0; i < Boxes; i++)
        {
            if (_grid[i] == Player.Empty)
            {
                // Copy the current grid
                Player[] newGrid = [.._grid];
                // Change the player at that cell
                newGrid[i] = player;

                yield return GetBoard(newGrid);
            }
        }
    }

    private Board GetBoard(Player[] newGrid)
    {
        var board = BoardRegistry.Find(newGrid);

        if (board == null)
        {
            board = new Board(newGrid);
            BoardRegistry.Add(board);
        }

        return board;
    }

    public void Render()
    {
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {Print(_grid[0])} | {Print(_grid[1])} | {Print(_grid[2])} |");
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {Print(_grid[3])} | {Print(_grid[4])} | {Print(_grid[5])} |");
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {Print(_grid[6])} | {Print(_grid[7])} | {Print(_grid[8])} |");
        Console.WriteLine("+---+---+---+\n");
    }

    private string Print(Player p)
    {
        if (p == Player.X) return "X";
        else if (p == Player.O) return "O";
        else return " ";        
    }

    public bool Equals(Board? board)
    {
        if (board == null)
            return false;

        return Equals(board._grid);
    }

    public bool Equals(Player[]? other)
    {
        if (other == null)
            return false;

        return _grid.SequenceEqual(other);
    }
}

// Create a registry of unique boards.
public static class BoardRegistry 
{
    private static readonly HashSet<Board> _boards = new();

    // Find an existing board by grid
    public static Board? Find(Player[] grid)
    {
        var board = _boards.FirstOrDefault(b => b.Equals(grid));
        if (board == null)
            Console.WriteLine("Board not found");
        return board;
    }

    public static bool Add(Board board)
    {
        Console.WriteLine($"Adding board {_boards.Count()}");
        return _boards.Add(board);
    }
}
