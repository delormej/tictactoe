Player player = Player.O;

const int TrainingRuns = 20;

Console.WriteLine("Press any key to start...");
Console.ReadKey();

Console.WriteLine("Training...");
Train();

Console.WriteLine("Ready to play...");
Console.ReadKey();

do 
{
    player = Player.O;
    PlayInteractive();
    Console.ReadLine();

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
            Console.Write("Enter move: 0-8: ");
            var key = Console.ReadKey();
            Console.WriteLine();

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
    
    RenderWinner(game);

    return game;
}

void Train()
{
    for (int i = 0; i < TrainingRuns; i++)
    {
        Play();
    }
}

void RenderWinner(Game game)
{
    if (game.Winner == Player.Empty) 
        Console.WriteLine("Draw!");
    else
        Console.WriteLine($"Player {game.Winner} wins!");
    
    game.Render();
}
