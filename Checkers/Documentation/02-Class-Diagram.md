# Class Diagram - Checkers Game

Detta diagram visar huvudklasserna och deras relationer.

```mermaid
classDiagram
    %% Game Logic Layer
    class GameService {
        -IRuleSetFactory _ruleSetFactory
        -IBoard _board
        -Player _player1
        -Player _player2
        -Player _currentPlayer
        -IMoveValidator _moveValidator
        -IGameHistory _gameHistory
        -PieceOperationsService _pieceOperationsService
        -GameStatus _gameStatus
        -bool _isInMultiJump
        +IRuleSet RuleSet
        +InitializeGame(string, string) bool
        +StartGame() void
        +MakeMove(Position, Position) bool
        +Undo() bool
        +CheckWinner() Player
        +GetValidMovesForPiece(Position) List~Position~
        +EndTurn() void
    }

    class Board {
        -Piece[,] squares
        +int Size
        +Initialize() void
        +GetPiece(Position) Piece
        +PlacePiece(Piece, Position) void
        +RemovePiece(Position) void
        +MovePiece(Position, Position) void
        +GetAllPieces() List~Piece~
        +GetAllPieces(PieceColor) List~Piece~
        +CountPieces(PieceColor) int
        +Clone() IBoard
    }

    class Piece {
        <<abstract>>
        +PieceColor Color
        +Position Position
        +bool IsKing
        +GetValidMoves(IBoard)* List~Position~
        +Clone()* Piece
    }

    class RegularPiece {
        +GetValidMoves(IBoard) List~Position~
        +Clone() Piece
        -GetForwardMoves(IBoard) List~Position~
        -GetCaptureMoves(IBoard) List~Position~
    }

    class KingPiece {
        -DirectionRows int[]
        -DirectionCols int[]
        +GetValidMoves(IBoard) List~Position~
        +Clone() Piece
        -GetAllDirectionMoves(IBoard) List~Position~
        -GetAllDirectionCaptures(IBoard) List~Position~
    }

    class MoveValidator {
        -IRuleSet _ruleSet
        +ValidateMove(Position, Position, IBoard, Player) bool
        +IsCapture(Position, Position) bool
        +GetCapturedPosition(Position, Position) Position?
        +HasValidMoves(Player, IBoard) bool
    }

    class PieceOperationsService {
        -IRuleSet _ruleSet
        -IMoveValidator _moveValidator
        +HandleCapture(Position, Position, IBoard) Piece?
        +PromoteToKing(Position, Piece, IBoard) void
        +IsPromotionPosition(Position, PieceColor) bool
    }

    class RuleSet {
        +int BoardSize
        +bool ForcedCaptures
        +bool AllowMultipleJumps
    }

    class Player {
        +string Name
        +PieceColor Color
    }

    class Position {
        <<struct>>
        +int Row
        +int Column
        +IsValid(int) bool
        +Equals(Position) bool
    }

    class GameHistory {
        -IBoard _initialBoard
        -List~Move~ _moves
        +RecordMove(Move) void
        +GetAllMoves() List~Move~
        +Undo(IBoard) bool
    }

    class Move {
        +Position From
        +Position To
        +Piece CapturedPiece
        +bool WasPromoted
        +int MoveNumber
        +DateTime TimeStamp
    }

    %% Presentation Layer
    class GameScreen {
        -IScreenChanger _screenChanger
        -GameService _gameService
        -Button btnStartGame
        -Button btnUndoMove
        -Button btnEndTurn
        -Position selectedPosition
        -List~Position~ validMoves
        -bool isPieceSelected
        +LoadContent(IAppContext) void
        +Update(GameTime) void
        +Draw(GameTime) void
    }

    class ScreenManager {
        -Dictionary~ScreenID, IScreen~ _screens
        -IScreen _currentScreen
        +AddScreen(ScreenID, IScreen) void
        +ChangeScreen(ScreenID) void
        +Update(GameTime) void
        +Draw(GameTime) void
    }

    class WindowComponent {
        <<abstract>>
        +Rectangle WindowRectangle
        +bool IsVisible
        +bool IsMouseOver
        +bool Enabled
        +event EventHandler Clicked
        +LoadContent(ContentManager) void
        +Update(GameTime) void
        +Draw(SpriteBatch) void
    }

    class Button {
        -Texture2D buttonTexture
        -SpriteFont buttonFont
        +string Text
        +LoadContent(ContentManager) void
        +Update(GameTime) void
        +Draw(SpriteBatch) void
    }

    %% Interfaces
    class IBoard {
        <<interface>>
        +Size int
        +GetPiece(Position) Piece
        +PlacePiece(Piece, Position) void
        +MovePiece(Position, Position) void
        +GetAllPieces() List~Piece~
        +Clone() IBoard
    }

    class IMoveValidator {
        <<interface>>
        +ValidateMove(Position, Position, IBoard, Player) bool
        +IsCapture(Position, Position) bool
        +GetCapturedPosition(Position, Position) Position?
        +HasValidMoves(Player, IBoard) bool
    }

    class IRuleSet {
        <<interface>>
        +BoardSize int
        +ForcedCaptures bool
        +AllowMultipleJumps bool
    }

    class IScreen {
        <<interface>>
        +LoadContent(IAppContext) void
        +UnloadContent() void
        +Update(GameTime) void
        +Draw(GameTime) void
    }

    %% Enums
    class PieceColor {
        <<enumeration>>
        Light
        Dark
    }

    class GameStatus {
        <<enumeration>>
        WaitingToStart
        InProgress
        Completed
        Abandoned
    }

    %% Relationships - Core Logic
    GameService --> IBoard : använder
    GameService --> IMoveValidator : använder
    GameService --> IRuleSet : använder
    GameService --> Player : har 2
    GameService --> GameHistory : använder
    GameService --> PieceOperationsService : använder
    GameService --> GameStatus : har

    Board ..|> IBoard : implementerar
    Board --> Piece : innehåller

    Piece <|-- RegularPiece : ärver
    Piece <|-- KingPiece : ärver
    Piece --> Position : har
    Piece --> PieceColor : har

    MoveValidator ..|> IMoveValidator : implementerar
    MoveValidator --> IRuleSet : använder

    RuleSet ..|> IRuleSet : implementerar

    PieceOperationsService --> IRuleSet : använder
    PieceOperationsService --> IMoveValidator : använder

    GameHistory --> Move : innehåller
    GameHistory --> IBoard : använder
    Move --> Position : har
    Move --> Piece : har

    %% Relationships - Presentation
    GameScreen ..|> IScreen : implementerar
    GameScreen --> GameService : använder
    GameScreen --> WindowComponent : innehåller

    ScreenManager --> IScreen : hanterar

    Button --|> WindowComponent : ärver

    %% Styling
    style GameService fill:
    style Board fill:
    style Piece fill:
    style MoveValidator fill:
    style GameScreen fill:
    style ScreenManager fill:
```

## Förklaring av klassrelationer

### Core Game Logic 
- **GameService**: Huvudklassen som koordinerar spellogiken (Facade pattern)
- **Board**: Hanterar brädets state och pjäser
- **MoveValidator**: Validerar drag enligt spelregler
- **PieceOperationsService**: Hanterar capture och promotion

### Piece Hierarchy 
- **Piece**: Abstrakt basklass
- **RegularPiece**: Standard pjäs (rör sig bara framåt)
- **KingPiece**: Dam (rör sig åt alla håll)

### Presentation Layer 
- **GameScreen**: Huvudskärmen för gameplay
- **ScreenManager**: Hanterar växling mellan screens
- **WindowComponent**: Basklass för UI-komponenter

### Design Patterns som syns:
1. **Strategy Pattern**: Piece-hierarkin
2. **Facade Pattern**: GameService
3. **Factory Pattern**: RuleSetFactory (ej visat)
4. **Observer Pattern**: WindowComponent events
5. **Dependency Injection**: Interfaces används överallt
