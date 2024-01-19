Player player = Player.O;

const int TrainingRuns = 500;

Train();

Console.WriteLine("Ready to play...");
Console.ReadKey();

do 
{
    PlayInteractive();
} while (true);


void PlayInteractive()
{
    Game game = new();
    game.Render();

    while (!game.IsGameOver)
    {
        try
        {
            // Play move for X
            Console.WriteLine("Enter move: 0-8");
            var key = Console.ReadKey();

            int move = int.Parse(key.KeyChar.ToString());
            game.Move(Player.X, move);

            if (!game.IsGameOver)
                // Play move for O
                game.Move(Player.O);

            game.Render();
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid move: " + e.Message);
        }
    }

    if (game.Winner == Player.Empty) 
        Console.WriteLine("Draw!");
    else
        Console.WriteLine($"Player {game.Winner} wins!");
}

Game Play()
{
    Game game = new();
    while (!game.IsGameOver)
    {
        // Alternate player
        player = (Player)((int)player *-1);
        game.Move(player);
    }
    
    if (game.Winner == Player.Empty) 
        Console.WriteLine("Draw!");
    else
        Console.WriteLine($"Player {game.Winner} wins!");
    
    game.Render();

    return game;
}

void Train()
{
    Console.WriteLine("Training...");

    for (int i = 0; i < TrainingRuns; i++)
    {
        Game game = new();
        while (!game.IsGameOver)
        {
            // Alternate player
            player = (Player)((int)player *-1);
            game.Move(player);
        }
    }
}
