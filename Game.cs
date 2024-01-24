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

    public void Move(Player player)
    {
        if (IsGameOver)
            throw new Exception("Game is over");

        var move = _board.Move(player);
        
        if (move == null || move.IsGameOver)
        {
            IsGameOver = true;
            
            if (move?.IsWin ?? false)
                _winner = player;

            Learn(player);
        }

        if (move != null)
            _moves.Add(move);
    }

    public void Render()
    {
        _board.Render();
    }

    private void Learn(Player player)
    {
        _moves.ForEach(m => 
        {
            if (_board.IsDraw)
            {
                m.Record(false);
            }
            else if (_winner == player && m.PlayedBy == player)
            {
                m.Record(true);
            }

            m.ClearMove();
        });
    }
}

