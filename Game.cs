public enum Player { X = 1, O = -1, Empty = 0 }


public class Game
{
    public Game()
    {
        _moves = new([Board.StartingBoard]);
    }

    List<Board> _moves;
    Player _winner = Player.Empty;
    Board _board => _moves.Last();
    public Player Winner => _winner;
    public bool IsGameOver = false;

    public Board Move(Player player)
    {
        if (IsGameOver)
            throw new Exception("Game is over");

        var move = _board.Move(player);
        
        if (move.IsGameOver)
        {
            IsGameOver = true;
            _moves.ForEach(move => move.Learn());

            if (move.IsWin)
                _winner = player;
        }

        _moves.Add(move);

        return move;
    }

    public void Render()
    {
        _board.Render();
    }
}

