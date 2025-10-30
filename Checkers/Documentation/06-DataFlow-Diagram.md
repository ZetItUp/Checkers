# Data Flow Diagram - Checkers Game

## 1. Overall Data Flow

```mermaid
graph TD
    subgraph "Input Sources"
        User["User Input - Mouse Clicks"]
        ConfigFile["RuleConfig.json - Spelregler"]
        SavedGames["Saved Games - JSON Files"]
    end

    subgraph "Presentation Layer"
        Mouse[MouseHelper]
        GameScreen[GameScreen]
        UI["UI Components - Buttons, Labels"]
    end

    subgraph "Business Logic"
        GameService[GameService]
        Validator[MoveValidator]
        Board["Board State"]
        Pieces["Pieces - RegularPiece, KingPiece"]
        Rules[RuleSet]
    end

    subgraph "Data Storage"
        History["GameHistory - Move List"]
        Persistence["GamePersistence - Save/Load"]
    end

    subgraph "Output"
        Display["Screen Rendering - SpriteBatch"]
        Sound["Audio Output - Win Sound"]
        SaveOutput["Saved Game Files - JSON"]
    end

    %% Input Flow
    User -->|Click Events| Mouse
    Mouse -->|Position Data| GameScreen
    ConfigFile -->|Load Rules| Rules
    SavedGames -->|Load Game| Persistence

    %% Presentation to Logic
    GameScreen -->|MakeMove| GameService
    GameScreen -->|GetValidMoves| GameService
    GameScreen -->|Undo| GameService

    %% Business Logic Flow
    GameService -->|ValidateMove| Validator
    GameService -->|Update State| Board
    GameService -->|Record Move| History
    Validator -->|Check Rules| Rules
    Board -->|Query Pieces| Pieces
    Pieces -->|Movement Logic| Validator

    %% Data Storage Flow
    History -->|All Moves| GameService
    GameService -->|Save Game| Persistence
    Persistence -->|Serialize| SaveOutput

    %% Output Flow
    GameService -->|Game State| GameScreen
    Board -->|Piece Positions| GameScreen
    GameScreen -->|Render| Display
    GameService -->|Victory Event| Sound

    style User fill: 
    style GameScreen fill: 
    style GameService fill: 
    style Board fill: 
    style Display fill: 
    style SaveOutput fill: 
```

---

## 2. Move Execution Data Flow

```mermaid
graph LR
    A["User Click - Position from"] -->|Mouse Event| B[GameScreen]
    B -->|GetValidMoves| C[GameService]
    C -->|Get Piece| D[Board]
    D -->|Piece Data| C
    C -->|GetValidMoves| E[Piece]
    E -->|Possible Positions| C
    C -->|Validate Each| F[MoveValidator]
    F -->|Filtered List| C
    C -->|Valid Moves| B
    B -->|Display| G[UI Rendering]

    H["User Click - Position to"] -->|Mouse Event| B
    B -->|MakeMove| C

    C -->|ValidateMove| F
    F -->|Check Rules| I[RuleSet]
    I -->|Rules Data| F
    F -->|Valid: true| C

    C -->|HandleCapture| J[PieceOperations]
    J -->|GetCapturedPos| F
    F -->|Position| J
    J -->|RemovePiece| D
    D -->|Piece Removed| J

    C -->|MovePiece| D
    D -->|Update Array| D

    C -->|IsPromotion| J
    J -->|PromoteToKing| D
    D -->|Replace Piece| D

    C -->|RecordMove| K[GameHistory]
    K -->|Store Move| K

    C -->|CheckWinner| F
    F -->|Winner or null| C

    C -->|Result| B
    B -->|Update UI| G

    style B fill:
    style C fill:
    style D fill:
    style F fill:
    style G fill:
```

---

## 3. Data Entity Relationship

```mermaid
erDiagram
    GAMESERVICE ||--|| BOARD : "contains"
    GAMESERVICE ||--|| MOVEVALIDATOR : "uses"
    GAMESERVICE ||--|| RULESET : "has"
    GAMESERVICE ||--o{ PLAYER : "has 2"
    GAMESERVICE ||--|| GAMEHISTORY : "maintains"
    GAMESERVICE ||--|| PIECEOPERATIONS : "uses"

    BOARD ||--o{ PIECE : "contains"
    PIECE ||--|| POSITION : "has"
    PIECE ||--|| PIECECOLOR : "has"

    PIECE ||--|| REGULARPIECE : "is-a"
    PIECE ||--|| KINGPIECE : "is-a"

    GAMEHISTORY ||--o{ MOVE : "stores"
    MOVE ||--|| POSITION : "has from"
    MOVE ||--|| POSITION : "has to"
    MOVE ||--o| PIECE : "may have captured"

    MOVEVALIDATOR ||--|| RULESET : "uses"
    PIECEOPERATIONS ||--|| RULESET : "uses"
    PIECEOPERATIONS ||--|| MOVEVALIDATOR : "uses"

    PLAYER ||--|| PIECECOLOR : "has"

    GAMESERVICE {
        GameStatus status
        bool isInMultiJump
    }

    BOARD {
        int size
        Piece squares
    }

    PIECE {
        PieceColor color
        Position position
        bool isKing
    }

    POSITION {
        int row
        int column
    }

    MOVE {
        Position from
        Position to
        Piece capturedPiece
        bool wasPromoted
        DateTime timestamp
        int moveNumber
    }

    RULESET {
        int boardSize
        bool forcedCaptures
        bool allowMultipleJumps
    }

    PLAYER {
        string name
        PieceColor color
    }
```

---

## 4. State Data Flow

```mermaid
graph TB
    subgraph "Game State"
        GameStatus["GameStatus Enum"]
        CurrentPlayer["Current Player - Player 1 or 2"]
        MultiJump["Multi-Jump State - bool"]
    end

    subgraph "Board State"
        PieceArray["2D Array - Piece array"]
        PieceList["Active Pieces - List of Pieces"]
    end

    subgraph "History State"
        MoveList["Move History - List of Move"]
        InitialBoard["Initial Board - Board Clone"]
    end

    subgraph "UI State"
        SelectedPiece["Selected Position"]
        ValidMoves["Valid Move List"]
        ButtonStates["Button Enable States"]
    end

    subgraph "Rule State"
        RuleConfig["RuleSet - From JSON"]
    end

    %% State changes
    GameStatus -->|StartGame| CurrentPlayer
    CurrentPlayer -->|MakeMove| PieceArray
    PieceArray -->|Update| PieceList
    PieceArray -->|RecordMove| MoveList

    MultiJump -->|After Capture| CurrentPlayer
    MultiJump -->|Affects| ValidMoves

    MoveList -->|Undo| PieceArray
    InitialBoard -->|Undo Base| PieceArray

    SelectedPiece -->|GetValidMoves| ValidMoves
    GameStatus -->|Enable/Disable| ButtonStates

    RuleConfig -->|Defines| GameStatus
    RuleConfig -->|Controls| MultiJump

    style GameStatus fill: 
    style PieceArray fill: 
    style MoveList fill: 
    style SelectedPiece fill: 
    style RuleConfig fill: 
```

---

## 5. Persistence Data Flow

```mermaid
sequenceDiagram
    participant User
    participant UI as GameScreen
    participant GS as GameService
    participant Persist as GamePersistence
    participant File as JSON File

    %% Save Flow
    Note over User,File: Save Game Flow
    User->>UI: Click "Save Game"
    UI->>Persist: SaveGame(gameService)
    Persist->>GS: Get Board
    GS-->>Persist: Board State
    Persist->>GS: Get History
    GS-->>Persist: Move List
    Persist->>GS: Get Players
    GS-->>Persist: Player Data
    Persist->>GS: Get RuleSet
    GS-->>Persist: Rules

    Persist->>Persist: Create GameSaveData object
    Persist->>Persist: JsonSerializer.Serialize()
    Persist->>File: Write JSON
    File-->>Persist: File saved
    Persist-->>UI: filename
    UI-->>User: "Game saved!"

    %% Load Flow
    Note over User,File: Load Game Flow
    User->>UI: Select saved game
    UI->>Persist: LoadGame(filename)
    Persist->>File: Read JSON
    File-->>Persist: JSON string
    Persist->>Persist: JsonSerializer.Deserialize()
    Persist->>Persist: Create new GameService
    Persist->>Persist: Restore Board State
    Persist->>Persist: Restore History
    Persist->>Persist: Restore Players
    Persist-->>UI: GameService
    UI->>GS: Use restored GameService
    UI-->>User: "Game loaded!"
```

---

## 6. Configuration Data Flow

```mermaid
graph TD
    A[RuleConfig.json] -->|File Read| B[RuleSetFactory]
    B -->|Parse JSON| C[JsonDocument]
    C -->|Extract Values| D{Valid Data?}

    D -->|Yes| E[Create RuleSet]
    D -->|No| F[Use Defaults]

    E --> G[RuleSet Object]
    F --> G

    G -->|BoardSize| H[Board.Initialize]
    G -->|ForcedCaptures| I[MoveValidator]
    G -->|AllowMultipleJumps| J[GameService]

    H -->|Create 8x8| K[Board State]
    I -->|Validate Moves| L[Move Logic]
    J -->|Multi-Jump Check| L

    G -->|Display| M[UI Labels]

    style A fill:
    style G fill:
    style K fill:
    style L fill:
    style M fill:
```

---

## 7. Event Data Flow

```mermaid
graph LR
    subgraph "Input Events"
        MouseMove[Mouse Move Event]
        MouseClick[Mouse Click Event]
        MouseRelease[Mouse Release Event]
    end

    subgraph "UI Event Processing"
        MouseHelper["MouseHelper - Position, State"]
        WindowComp["WindowComponent - IsMouseOver"]
        Button["Button - Clicked Event"]
    end

    subgraph "Game Event Handlers"
        BtnStart[BtnStartGame_Clicked]
        BtnUndo[BtnUndoMove_Clicked]
        BtnEnd[BtnEndTurn_Clicked]
        BtnSave[BtnSaveGame_Clicked]
    end

    subgraph "Game Actions"
        StartGame[GameService.StartGame]
        UndoMove[GameService.Undo]
        EndTurn[GameService.EndTurn]
        SaveGame[GamePersistence.Save]
    end

    subgraph "State Changes"
        StatusChange[GameStatus Change]
        BoardUpdate[Board Update]
        UIUpdate[UI Refresh]
    end

    MouseMove --> MouseHelper
    MouseClick --> MouseHelper
    MouseRelease --> MouseHelper

    MouseHelper --> WindowComp
    WindowComp --> Button

    Button -->|Event Fire| BtnStart
    Button -->|Event Fire| BtnUndo
    Button -->|Event Fire| BtnEnd
    Button -->|Event Fire| BtnSave

    BtnStart --> StartGame
    BtnUndo --> UndoMove
    BtnEnd --> EndTurn
    BtnSave --> SaveGame

    StartGame --> StatusChange
    UndoMove --> BoardUpdate
    EndTurn --> StatusChange

    StatusChange --> UIUpdate
    BoardUpdate --> UIUpdate

    style MouseHelper fill:
    style Button fill:
    style StartGame fill:
    style UIUpdate fill:
```

---

## Data Transformation Examples

### 1. User Click → Board Position
```
Mouse Click (pixels) → MouseHelper → Position (pixels)
Position (pixels) ÷ drawScale → Position (grid)
Position (grid) → Position(row, col) struct
```

### 2. Move → History → Undo
```
MakeMove(from, to) → Move object created
Move object → GameHistory.RecordMove()
Move object → List<Move> stored
Undo() → Replay all moves except last
Replayed moves → Board reconstructed
```

### 3. Configuration → Rules → Validation
```
RuleConfig.json → RuleSetFactory.CreateFromJsonFile()
JSON values → RuleSet properties
RuleSet → MoveValidator constructor
RuleSet.ForcedCaptures → ValidateMove logic
```

### 4. Piece → Rendering
```
Piece.Color + Piece.IsKing → Select Texture
Piece.Position → Calculate pixel coordinates
Pixel coords × drawScale → Screen position
Screen position + Texture → SpriteBatch.Draw()
```

---

## Key Data Stores

| Store | Type | Persistence | Owner |
|-------|------|-------------|-------|
| Board State | Piece?[8,8] | Volatile | Board |
| Move History | List\<Move\> | Volatile | GameHistory |
| Game Configuration | RuleSet | File (JSON) | RuleSetFactory |
| Saved Games | GameSaveData | File (JSON) | GamePersistence |
| UI State | Various bools | Volatile | GameScreen |
| Player Info | Player objects | Volatile | GameService |

---

## Data Flow Performance Notes

###  Efficient:
- **Direct array access**: `squares[row, col]` - O(1)
- **Piece movement**: Only updates affected cells
- **Validation caching**: Valid moves calculated once per selection

###  Moderate:
- **GetAllPieces()**: Iterates entire board - O(n²) but n=8 så 64 operationer
- **HasValidMoves()**: Checks all pieces and their moves - O(pieces × moves)

###  Inefficient:
- **Undo via replay**: Replays all moves - O(n) where n = number of moves
- **Victory check every frame**: Could be optimized to only check after moves
- **Board.Clone()**: Copies entire 8x8 array even for empty cells

**Recommendation**: För detta projekt är inefficienserna acceptabla. Märks inte på små bräden och korta spel.