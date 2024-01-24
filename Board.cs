public class Board
{
    const int Boxes = 9; 

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
    public Board? Move(Player player)
    {
        if (_move != null) 
            throw new Exception("Move already played");

        if (IsGameOver)
            throw new Exception("Game is over");

        _player = player;

        LoadMoves();

        var moves = _player == Player.X ? _movesForX : _movesForO;

        if (moves!.Count == 0)
        {
            // Ran out of moves, game is over.
            return null;
        }

        // Save the move so that we can record win or draw.
        _move = moves[_random.Next(0, moves.Count)];
        
        // remove from available moves
        moves.Remove(_move);

        return _move;
    }

    /// <summary>
    /// Player plays a move at the given position.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="position"></param>
    /// <returns>Board with the new move or null if invalid.</returns>
    public Board? Move(Player player, int position)
    {
        _player = player;

        if (position < 0 || position >= Boxes)
            throw new Exception("Invalid position");

        if (_grid[position] != Player.Empty)
            throw new Exception("Position already taken");

        Player[] newGrid = [.._grid];
        newGrid[position] = player;

        LoadMoves();

        var moves = _player == Player.X ? _movesForX : _movesForO;

        // Don't create a new object, find existing.
        foreach (var move in moves!)
        {
            if (move._grid.SequenceEqual(newGrid))
            {
                _move = move;
                return _move;
            }
        }
        
        return null;
    }

    public bool IsGameOver => IsWin || IsDraw;
    
    public bool IsWin => _isWin;

    public bool IsDraw => _isDraw;

    public Player? PlayedBy => _player;

    /// <summary>
    /// Reinforces wins & draws, and penalizes losses.
    /// </summary>
    /// <exception cref="Exception">Move not played yet</exception>
    public void Record(bool isWin)
    {
        if (_move == null)
        {
            Console.WriteLine($"Move not played! {this.GetHashCode()}");

            return;
            // throw new Exception("Move not played");
        }

        var moves = _player == Player.X ? _movesForX : _movesForO;

        if (moves == null)
            throw new Exception("No available moves");

        // Add 2 for a Win, 1 for a Draw, 0 for a Loss
        int count = isWin ? 2 : 1;

        // Don't learn for Player O
        // if (_player == Player.O) count = 1;

        for (int i = 0; i < count; i++)
        {
            moves.Add(_move);
        }

        Console.WriteLine($"Learned {count} moves for {_player}");
    }

    public void ClearMove()
    {
        // Clear the move for the next learning opportunity.
        _move = null;
        _player = null;
    }

    private void LoadMoves()
    {
        if (_movesForX == null)
            _movesForX = GetAvailableMoves(Player.X).ToList();
        if (_movesForO == null)            
            _movesForO = GetAvailableMoves(Player.O).ToList();        
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

                yield return new Board(newGrid);
            }
        }
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
}
