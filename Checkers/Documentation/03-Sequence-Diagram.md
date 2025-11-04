# Sequence Diagrams - Checkers Game

## 1. Gör ett vanligt drag (utan capture)

Detta visar flödet när en spelare gör ett vanligt drag.

```mermaid
sequenceDiagram
    actor Player
    participant GS as GameScreen
    participant GameService
    participant MV as MoveValidator
    participant Board
    participant POS as PieceOperationsService
    participant History as GameHistory

    Player->>GS: Klickar på pjäs (Position from)
    GS->>GameService: GetValidMovesForPiece(from)
    GameService->>Board: GetPiece(from)
    Board-->>GameService: Piece
    GameService->>Piece: GetValidMoves(board)
    Piece-->>GameService: List<Position>
    GameService->>MV: ValidateMove för varje position
    MV-->>GameService: Filtrerade giltiga drag
    GameService-->>GS: List<Position> validMoves
    GS->>GS: Visa giltiga drag på UI

    Player->>GS: Klickar på giltig position (to)
    GS->>GameService: MakeMove(from, to)

    GameService->>MV: ValidateMove(from, to, board, player)
    MV->>Board: GetPiece(from)
    Board-->>MV: Piece
    MV->>MV: IsPieceOwnedByPlayer()
    MV->>MV: IsDestinationEmpty()
    MV->>MV: IsMoveInValidList()
    MV-->>GameService: true

    GameService->>POS: HandleCapture(from, to, board)
    POS-->>GameService: null (inget capture)

    GameService->>Board: MovePiece(from, to)
    Board->>Board: Uppdatera squares array
    Board->>Piece: Position = to
    Board-->>GameService: Flyttad

    GameService->>Board: GetPiece(to)
    Board-->>GameService: Piece
    GameService->>POS: IsPromotionPosition(to, color)
    POS-->>GameService: false

    GameService->>History: RecordMove(move)
    History-->>GameService: Sparat

    GameService->>GameService: CheckWinner()
    GameService->>GameService: SwitchTurn()
    GameService-->>GS: true

    GS->>GS: Uppdatera UI
    GS-->>Player: Drag utfört, turen växlar
```

---

## 2. Gör ett capture-drag (med promotion till King)

```mermaid
sequenceDiagram
    actor Player
    participant GS as GameScreen
    participant GameService
    participant MV as MoveValidator
    participant Board
    participant POS as PieceOperationsService
    participant History as GameHistory

    Player->>GS: Klickar på position (capture move)
    GS->>GameService: MakeMove(from, to)

    GameService->>MV: ValidateMove(from, to, board, player)
    MV->>MV: IsCapture(from, to) = true
    MV->>MV: GetCapturedPosition(from, to)
    MV->>Board: GetPiece(capturedPosition)
    Board-->>MV: Opponent Piece
    MV->>MV: Validera att det är motståndarens pjäs
    MV-->>GameService: true

    GameService->>POS: HandleCapture(from, to, board)
    POS->>MV: GetCapturedPosition(from, to)
    MV-->>POS: Position
    POS->>Board: GetPiece(capturedPosition)
    Board-->>POS: Captured Piece
    POS->>Board: RemovePiece(capturedPosition)
    Board->>Board: squares[pos] = null
    POS-->>GameService: Captured Piece

    GameService->>Board: MovePiece(from, to)
    Board-->>GameService: Flyttad

    GameService->>Board: GetPiece(to)
    Board-->>GameService: Piece
    GameService->>POS: IsPromotionPosition(to, color)
    POS-->>GameService: true (nått motståndares basrad)

    GameService->>POS: PromoteToKing(to, piece, board)
    POS->>Board: RemovePiece(to)
    POS->>POS: Skapa ny KingPiece
    POS->>Board: PlacePiece(kingPiece, to)
    POS-->>GameService: Befordrad

    GameService->>History: RecordMove(move med WasPromoted=true)

    GameService->>GameService: CheckWinner()
    GameService->>GameService: SwitchTurn()
    GameService-->>GS: true

    GS->>GS: Visa King texture
    GS-->>Player: Pjäs tagen och befordrad till King!
```

---

## 3. Multi-Jump Capture Sequence

```mermaid
sequenceDiagram
    actor Player
    participant GS as GameScreen
    participant GameService
    participant MV as MoveValidator
    participant Board

    Note over Player,Board: AllowMultipleJumps = true

    Player->>GS: Gör första capture-drag
    GS->>GameService: MakeMove(from1, to1)
    GameService->>MV: ValidateMove()
    MV-->>GameService: true
    GameService->>Board: MovePiece(from1, to1)
    GameService->>GameService: HandleCapture (tar pjäs 1)

    GameService->>GameService: CanPieceCaptureAgain(to1)?
    GameService->>Piece: GetValidMoves(board)
    Piece-->>GameService: Possible moves
    GameService->>MV: IsCapture för varje move
    MV-->>GameService: Hittar fler captures
    GameService->>GameService: _isInMultiJump = true
    GameService-->>GS: true (men ingen turn switch!)

    Note over GS: Samma spelare har fortfarande tur
    GS->>GS: Auto-select pjäs på to1
    GS->>GameService: GetValidMovesForPiece(to1)
    GameService->>GameService: Filtrerar BARA capture-moves
    GameService-->>GS: Bara capture-drag visas

    Player->>GS: MÅSTE göra capture (or EndTurn om tillåtet)
    GS->>GameService: MakeMove(to1, to2)
    GameService->>Board: MovePiece(to1, to2)
    GameService->>GameService: HandleCapture (tar pjäs 2)

    GameService->>GameService: CanPieceCaptureAgain(to2)?
    GameService-->>GameService: false (inga fler captures)

    GameService->>GameService: _isInMultiJump = false
    GameService->>GameService: SwitchTurn()
    GameService-->>GS: true

    GS-->>Player: Multi-jump slutförd! 2 pjäser tagna
```

---

## 4. Undo Operation

```mermaid
sequenceDiagram
    actor Player
    participant GS as GameScreen
    participant GameService
    participant History as GameHistory
    participant Board

    Player->>GS: Klickar "Undo Move"
    GS->>GameService: Undo()

    GameService->>History: Undo(board)
    History->>History: GetAllMoves()
    History->>History: Ta bort sista move från lista

    History->>Board: Clone initial board
    Board-->>History: Clean board copy

    loop För varje move (utom sista)
        History->>Board: MovePiece(move.From, move.To)

        alt Om move hade capture
            History->>Board: PlacePiece(capturedPiece)
        end

        alt Om move hade promotion
            History->>Board: RemovePiece
            History->>Board: PlacePiece(KingPiece)
        end
    end

    History-->>GameService: true

    GameService->>GameService: SwitchTurn()
    GameService-->>GS: true

    GS->>GS: Uppdatera UI
    GS-->>Player: Drag ångrat
```

---

## 5. Spara och Ladda Spel

```mermaid
sequenceDiagram
    actor Player
    participant GS as GameScreen
    participant GameService
    participant GP as GamePersistence
    participant FS as FileSystem

    %% Save Flow
    Player->>GS: Klickar "Save Game"
    GS->>GP: SaveGame(gameService)
    GP->>GameService: Hämta alla properties
    GameService-->>GP: Board, History, Players, RuleSet
    GP->>GP: Serialisera till JSON
    GP->>FS: Skriv till Saves/game_timestamp.json
    FS-->>GP: Fil sparad
    GP-->>GS: Filename
    GS->>GS: Console.WriteLine("Game saved")
    GS-->>Player: Spel sparat!

    %% Load Flow (från Replay Screen)
    Player->>ReplayScreen: Väljer sparat spel
    ReplayScreen->>GP: LoadGame(filename)
    GP->>FS: Läs JSON-fil
    FS-->>GP: JSON data
    GP->>GP: Deserializa JSON
    GP->>GP: Skapa GameService
    GP->>GP: Återskapa Board
    GP->>GP: Återskapa History med moves
    GP-->>ReplayScreen: GameService
    ReplayScreen->>ReplayScreen: Visa/Spela upp moves
    ReplayScreen-->>Player: Spel laddat!
```

---

## Viktiga observationer:

### Validering sker på flera nivåer:
1. **Piece.GetValidMoves()**: Returnerar fysiskt möjliga drag (geometri)
2. **MoveValidator.ValidateMove()**: Kontrollerar spelregler (ownership, forced captures, etc)
3. **GameScreen**: Visar bara validerade drag för användaren

### Multi-Jump är event-driven:
- `CanPieceCaptureAgain()` kontrolleras EFTER varje capture
- `_isInMultiJump` flag håller state
- GameScreen auto-selekterar pjäsen för nästa capture

### Undo är ineffektivt men enkelt:
- Replay från början varje gång
- För små spel (< 100 drag) är detta okej
- Command Pattern skulle göra det O(1) istället för O(n)

### Separation of Concerns:
- **GameScreen**: Input och rendering
- **GameService**: Spellogik och koordinering
- **MoveValidator**: Regelvalidering
- **Board**: State management