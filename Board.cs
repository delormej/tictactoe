public enum Player : byte { X = 0b01, O = 0b10, Empty = 0b00 }

public struct BoardState(int _state)
{
    /*
        An array of bits, each space on a board is represented by 2 bits
        00 = Empty
        01 = Player X
        10 = Player O

        For example
        X X X
        O X O
        _ O _

        *0 *1 *2 *3 *4 *5 *6 *7 *8 // Position
        01 01 01 10 01 10 OO 01 00 // Representation
    */
    public BoardState AddMove(Player player, int position)
    {
        // Use bit shift by position
        int shift = (BoardSize -1) - position; 

        // TODO: ensure there isn't already a move here.

        int newState = ((int)player << shift) | _state; 

        return new BoardState(newState);
    }

    public Player PlayerAt(int position)
    {
        int offset = BoardOffset + (position * 2);
        return (Player)(_state >> offset);
    }

    public static string PlayerToString(Player player)
    {
        switch (player)
        {
            case Player.X:
                return "X";
            case Player.O:
                return "O";
            default:
                return "";
        }        
    }

    public const byte BoardSize = 9; 
    public static readonly int BoardOffset = 32-(BoardSize*2); // 2 bits per 
    // 0000 0001 1111 1111
    static readonly int BoardMask = (2^9)-1;      // 511 
    // 0000 0110 0000 0000

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

    public int Value => _state;

    public static void PrintWins(Player player)
    {
        for (int row = 0; row < WinVectors.Length; row++)
        {
            int i = ((int)player) >> (BoardOffset + WinVectors[row,0]);
            Console.WriteLine($"number: {i}");
        }

    }

    static readonly int[] WinMasks = {
        0b010101000000000000,
        0b000000010101000000,
        0b000000000000010101,

    };    

    public readonly static BoardState BoardEmpty = new(0);
}

public class Board
{
    const int Boxes = 9; 

    public static Board StartingBoard = new(BoardState.BoardEmpty);

    Board(BoardState boardState)
    {
        _boardState = boardState;
        _random = new();
        // _isWin = CalculateIsWin();
        // _isDraw = CalculateIsDraw();
        // _movesForX = GetAvailableMoves(Player.X).ToList();
        // _movesForO = GetAvailableMoves(Player.O).ToList();
    }

    Board? _move;
    Player? _player;
    readonly bool _isWin, _isDraw;
    readonly Random _random;
    readonly BoardState _boardState;
    List<Board>? _movesForX;
    List<Board>? _movesForO;

    public BoardState State => _boardState;

    /// <summary>
    /// Computer plays a move for the given player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Board? Move(Player player)
    {
        if (IsGameOver)
            throw new Exception("Game is over");

        _player = player;

        LoadMoves();

        // get a random, available move
        var moves = _player == Player.X ? _movesForX : _movesForO;

        if (moves == null)
            throw new ApplicationException("No moves were loaded!");


        if (moves.Count() <= 0)
        {
            return null;
        }

        _move = moves[_random.Next(0, moves.Count)];
        
        // Only remove if this is computer player.
        if (player == Player.O)
            moves.Remove(_move);

        return _move;
    }

    public Board Move(Player player, int position)
    {
        if (position < 0 || position >= Boxes)
            throw new Exception("Invalid position");

        _move = new Board(_boardState.AddMove(player, position));

        _player = player;

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
    /// Reinforces wins & draws, penalizes losses.
    /// </summary>
    /// <exception cref="Exception">Move not played yet</exception>
    public void Learn()
    {
        if (_player == Player.X)
            return;
            
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
    // private bool CalculateIsWin()
    // {
    //     for (int i = 0; i < WinVectors.GetLength(0); i++)
    //     {
    //         var sum = Math.Abs( 
    //             (int)_boardState[WinVectors[i, 0]] + 
    //             (int)_boardState[WinVectors[i, 1]] + 
    //             (int)_boardState[WinVectors[i, 2]]
    //         );

    //         if (sum == 3) return true;
    //     }
        
    //     return false;
    // }

    // /// <summary>
    // /// If all cells are filled, it's a draw.
    // /// </summary>
    // /// <returns></returns>
    // private bool CalculateIsDraw()
    // {
    //     var sum = _boardState.Cast<int>().Select(i => Math.Abs(i)).Sum();
    //     return sum == Boxes;
    // }

    private IEnumerable<Board> GetAvailableMoves(Player player)
    {
        for (int i = 0; i < BoardState.BoardSize; i++)
        {
            int offset = BoardState.BoardOffset + i*2;

            if ((_boardState.Value >> offset) == (int)Player.Empty)
            {
                var boardState = _boardState.AddMove(player, i);

                yield return new Board(boardState);
            }
        }
    }

    public void Render()
    {
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {BoardState.PlayerToString(_boardState.PlayerAt(0))} | {BoardState.PlayerToString(_boardState.PlayerAt(1))} | {BoardState.PlayerToString(_boardState.PlayerAt(2))} |");
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {BoardState.PlayerToString(_boardState.PlayerAt(3))} | {BoardState.PlayerToString(_boardState.PlayerAt(4))} | {BoardState.PlayerToString(_boardState.PlayerAt(5))} |");
        Console.WriteLine("+---+---+---+");
        Console.WriteLine($"| {BoardState.PlayerToString(_boardState.PlayerAt(6))} | {BoardState.PlayerToString(_boardState.PlayerAt(7))} | {BoardState.PlayerToString(_boardState.PlayerAt(8))} |");
        Console.WriteLine("+---+---+---+\n");
    }

    private string Print(Player p)
    {
        if (p == Player.X) return "X";
        else if (p == Player.O) return "O";
        else return " ";        
    }
}

// // Create a registry of unique boards.
// public static class BoardRegistry 
// {
//     private static readonly HashSet<Board> _boards = new();

//     // Find an existing board by grid
//     public static Board? Find(BoardState boardState)
//     {
//         if (_boards.Contains(boardState))


//         var board = _boards.FirstOrDefault(b => b.Equals(grid));
//         if (board == null)
//             Console.WriteLine("Board not found");
//         return board;
//     }

//     public static bool Add(Board board)
//     {
//         Console.WriteLine($"Adding board {_boards.Count()}");
//         return _boards.Add(board);
//     }
// }

public class BoardComparer : IEqualityComparer<Board>
{
    public bool Equals(Board x, Board y)
    {
        if (x?.State == null || y?.State == null)
            return false;

        return x.State.Value == y.State.Value;
    }

    public int GetHashCode(Board obj)
    {
        return obj.State.Value;
    }
}
