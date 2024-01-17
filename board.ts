// There is a universe of all possible board states 9^3 (729)
// However, you can't have a state where there is more than 1 more X than O and vice versa


// A Board represents the state of each cell and the set of possible moves (next Board)
//  Board has a method Move(player: Player)
//  Board.Move(player: Player) will return a new Board for a Player based on available moves
//  The Board will keep track of it's `move` which is immutable
// The Game hangs on to a list of Moves as returned from each Board.Move()
// If the Board was a winner:
//  Revisit each of the Moves

/*
    Board
        +makeMove(player: Player)
            // selects a random move from the available moves 
            // removes that move from the available moves
            // keeps a reference to the move
        +markWin()
            // adds 2 instances of the move to the available moves
        +markDraw()
            // adds the move back to the available moves
        -availableMoves (references to other Boards)
        -move

    Game
        +newGame() : Game
            // creates a new Game with the static Board.StartingBoard property
        +makeMove()
            // adds a move to the current list of game moves
            // this.moves.push(this.board.makeMove())
            // if this is a winner, iterate through all moves and call markWin()
            // if it's a draw, iterate through all moves and call markDraw() 
            //
            //  
        -board => this.moves.last
        -isWinner()
            this.board 
        -isDraw()


*/

let counter = 0;

enum Player { X = 1, O = -1, EMPTY = 0 }
type Row = [Player, Player, Player];
type BoardState = [Row, Row, Row];

const EmptyBoard: BoardState = [
    [Player.EMPTY, Player.EMPTY, Player.EMPTY],
    [Player.EMPTY, Player.EMPTY, Player.EMPTY],
    [Player.EMPTY, Player.EMPTY, Player.EMPTY]
];

/** 3x3 TicTacToe Board which represents Xs & Os. */
export class Board {
    public static readonly StartingBoard = new Board(EmptyBoard);

    public readonly id: number;

    private board: BoardState; // need to make this completely immutable
    private move?: Board;
    private winner = Player.EMPTY;
    private draw = false;

    // array for all possible moves by player.
    private movesForX: Board[] = [];
    private movesForO: Board[] = [];

    /**
     * Creates a new Board.
     */
    constructor(state: BoardState | null) {
        
        this.id = (++counter).valueOf();
        console.log('ctor()', this.id);

        // Copy the boardstate
        if (state) {
            this.board = state;
        }
        else {
            this.board = EmptyBoard;
        }
    }
    
    public getBoardState(): BoardState {
        return this.board;
    }

    public getWinner(): Player {
        return this.winner;
    }

    public isDraw(): boolean {
        return this.draw;
    }

    /**
    // selects a random move from the available moves 
    // removes that move from the available moves
    // keeps a reference to the move
    */
    public makeMove(player: Player) : Board {
        if (this.movesForO.length === 0 || this.movesForX.length === 0) {
            // Initialize available moves
            this.setAvailableMoves(Player.X);
            this.setAvailableMoves(Player.O);    
        }

        const moves = player === Player.X ? this.movesForX : this.movesForO;
        this.move = moves[Math.floor(Math.random() * moves.length)];

        this.setWinner();
        if (this.winner !== Player.EMPTY) {
            this.markWin(player);
        }
        else {
            this.setDraw();
            if (this.draw)
                this.markDraw(player);            
        }

        return this.move;
    }
 
    /** Gets the board state for all possible moves for a player. */
    private setAvailableMoves(player: Player) {
        this.board.forEach((row, rowIndex) => {
            // console.log('outer');
            row.forEach((col, colIndex) => {
                // console.log('inner');
                // if (set) return;
                if (col !== player) {
                    console.log('original', this.id);
                    display(this.board)
                    // Copy the current board and make the move.
                    let newBoardState = this.clone();
                    newBoardState[rowIndex][colIndex] = player;
                    
                    console.log('new', this.id);
                    display(newBoardState);

                    let newBoard = new Board(newBoardState);

                    // Add the new board to the available moves.
                    player === Player.X ? this.movesForX.push(newBoard) : 
                        this.movesForO.push(newBoard);

                    // Force exit of the method.
                    console.log('exit');
                    // set = true;
                    return;
                }
            })
        })
    }

    private clone() : BoardState {
        let cloned: BoardState = EmptyBoard;

        for (let r = 0; r < 3; r++)
            for (let c = 0; c < 3; c++) {
                // Ensure a copy
                let val = this.board[r][c].valueOf();
                cloned[r][c] = val;
            }

        return cloned;
    }

    private setDraw() {
        this.draw = this.board.every((row) => 
            row.every((cell) => cell !== Player.EMPTY));
    }

    private setWinner() {
        // Check rows
        for (let row = 0; row < this.board.length; row++) {
            this.winner = this.getWinnerPlayer(
                this.board[row][0] + this.board[row][1] + this.board[row][2]);
            if (this.winner !== Player.EMPTY)
                return;
        }

        // Check cols
        for (let col = 0; col < this.board.length; col++) {
            this.winner = this.getWinnerPlayer(
                this.board[0][col] + this.board[1][col] + this.board[2][col]);
            if (this.winner !== Player.EMPTY)
                return;
        }

        // Check top left to bottom right
        this.winner = this.getWinnerPlayer(
            this.board[0][0] + this.board[1][1] + this.board[2][2]);
        if (this.winner !== Player.EMPTY)
            return;

        // Check top right to bottom left
        this.winner = this.getWinnerPlayer(
            this.board[0][2] + this.board[1][1] + this.board[2][0]);
    }

    private getWinnerPlayer(sum : number): Player {
        if (sum === 3)
            return Player.X;
        if (sum === -3)
            return Player.O;

        return Player.EMPTY;
    }

    private markWin(player: Player) {
        this.learn(player, 2);
    }

    private markDraw(player: Player) {
        this.learn(player, 1);   
    }

    private learn(player: Player, count: number)
    {
        if (!this.move)
            throw new Error("No move has been made.");

        // Adds # instances of the move to the available moves

        const moves = player === Player.X ? this.movesForX : this.movesForO;
        for (let i = 0; i < count; i++)
            moves.push(this.move);
    }
}

export class Game {
    private _boards!: Board[];
    private _currentBoard!: Board;
    private _winner: Player = Player.EMPTY;
    private _gameOver: boolean = false;
    
    /**
     * Creates a new Game.
     */
    constructor() {
        this.reset();
    }

    public get winner() : Player {
        return this._winner;
    }

    public get gameOver() : boolean {
        return this._gameOver;
    }

    public get board() : BoardState {
        return this._currentBoard.getBoardState();
    }

    public play(player: Player) {
        this._currentBoard.makeMove(player)
        return;

        this._currentBoard = this._currentBoard.makeMove(player);
        this._boards.push(this._currentBoard);

        this._winner = this._currentBoard.getWinner();
        if (this._winner != Player.EMPTY || this._currentBoard.isDraw()) {
            this._gameOver = true;
        }
    }

    public reset() {
        this._currentBoard = Board.StartingBoard;
        this._boards = [this._currentBoard];
    }
}

//
// TEST
//
/** Writes out a board state to console. */
export function display(boardState: BoardState) {
    let rows = boardState.map((row) => 
    row.map((cell) => cell === -1 ? ' O' : cell === 1 ? ' X' : '  ')
        .join(' | '));

    console.log('\n+--+----+---+');
    console.log(rows.join('\n+--+----+---+\n'));
    console.log('+--+----+---+\n\n');
}   

let player = Player.O;
let plays = 0;
const game = new Game();
const maxPlays = 1;

(game.board);
console.log("And it begins...");

while (!game.gameOver && plays++ < maxPlays) {
    // alternate player
    game.play(player * -1); 
    display(game.board);
}

if (game.winner === Player.EMPTY) {
    console.log("It's a draw!");
}
else {
    console.log(`Player ${player} wins!`);
}
