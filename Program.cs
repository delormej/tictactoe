Player player = Player.O;

const int TrainingRuns = 15;

Console.WriteLine("Press any key to start...");
Console.ReadKey();

Console.WriteLine("Training...");
Train();

Console.WriteLine("Ready to play...");
Console.ReadKey();

do 
{
    player = Player.O;
    Play();
    Console.ReadLine();

} while (true);


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
