using System.Diagnostics.Metrics;

public class Game
{
    static Meter _meter = new Meter("tic_tac_toe");
    static Counter<int> _computerGamesWon = _meter.CreateCounter<int>("tic_tac_toe.computer_games_won");
    static Counter<int> _computerGamesLost = _meter.CreateCounter<int>("tic_tac_toe.computer_games_lost");

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
        
        return Move(player, move);
    }

    public Board Move(Player player, int position)
    {
        if (IsGameOver)
            throw new Exception("Game is over");

        var move = _board.Move(player, position);
        
        return Move(player, move);
    }

    private Board Move(Player player, Board move)
    {
        if (move.IsGameOver)
        {
            IsGameOver = true;
            _moves.ForEach(move => move.Learn());

            if (move.IsWin)
            {
                _winner = player;
                if (_winner == Player.O)
                    _computerGamesWon.Add(1);
                else
                    _computerGamesLost.Add(1);
            }
        }

        _moves.Add(move);

        return move;
    }

    public void Render()
    {
        _board.Render();
    }
}

