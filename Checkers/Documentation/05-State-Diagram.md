# State Diagrams - Checkers Game

## 1. Game Status State Machine

Detta visar de olika tillstånden som ett spel kan ha.

```mermaid
stateDiagram-v2
    [*] --> WaitingToStart: InitializeGame()

    WaitingToStart --> InProgress: StartGame()
    WaitingToStart --> [*]: Tillbaka till huvudmeny

    InProgress --> InProgress: MakeMove()
    InProgress --> InProgress: Undo()
    InProgress --> Completed: CheckWinner() returnerar spelare
    InProgress --> Abandoned: Användaren lämnar spelet
    InProgress --> WaitingToStart: RestartGame()

    Completed --> [*]: Tillbaka till huvudmeny
    Completed --> WaitingToStart: RestartGame()

    Abandoned --> [*]: Stäng spel

    note right of WaitingToStart
        - Board är initierat
        - Spelare är satta
        - Start-knapp är aktiv
    end note

    note right of InProgress
        - Drag kan göras
        - Undo är aktivt (om moves finns)
        - Turnering mellan spelare
        - Multi-jump möjligt
    end note

    note right of Completed
        - Vinnare är bestämd
        - Victory screen visas
        - Inga fler drag möjliga
    end note
```

---

## 2. Player Turn State Machine

Detta visar vad som händer under en spelares tur.

```mermaid
stateDiagram-v2
    [*] --> WaitingForPieceSelection: Spelarens tur börjar

    WaitingForPieceSelection --> PieceSelected: Klicka på egen pjäs
    WaitingForPieceSelection --> WaitingForPieceSelection: Klicka på motståndarpjäs (ignoreras)

    PieceSelected --> WaitingForPieceSelection: Klicka på annan pjäs
    PieceSelected --> MoveMade: Klicka på giltig move

    MoveMade --> CheckingForCapture: Evaluera drag

    CheckingForCapture --> CheckingForPromotion: Inget capture
    CheckingForCapture --> CapturePerformed: Capture gjort

    CapturePerformed --> CheckingForPromotion: Ingen multi-jump möjlig
    CapturePerformed --> MultiJumpAvailable: Fler captures möjliga

    MultiJumpAvailable --> PieceSelected: Auto-select samma pjäs
    note right of MultiJumpAvailable
        - AllowMultipleJumps = true
        - CanPieceCaptureAgain() = true
        - _isInMultiJump = true
    end note

    CheckingForPromotion --> PromotingToKing: Nått motståndarens basrad
    CheckingForPromotion --> CheckingForWinner: Ingen promotion

    PromotingToKing --> CheckingForWinner: King skapad

    CheckingForWinner --> GameOver: Vinnare hittad
    CheckingForWinner --> SwitchTurn: Ingen vinnare än

    SwitchTurn --> [*]: Motståndaren får tur

    GameOver --> [*]: Spelet slutar

    note right of PieceSelected
        - validMoves visas på UI
        - isPieceSelected = true
        - selectedPosition sparad
    end note

    note right of SwitchTurn
        - currentPlayer byts
        - _isInMultiJump = false
        - validMoves rensas
    end note
```

---

## 3. Move Validation State Flow

```mermaid
stateDiagram-v2
    [*] --> ValidatingMove: MakeMove(from, to) anropat

    ValidatingMove --> CheckPieceOwnership: Starta validering

    CheckPieceOwnership --> ValidationFailed: Pjäs ägs inte av spelare
    CheckPieceOwnership --> CheckDestination: Pjäs ägs av spelare

    CheckDestination --> ValidationFailed: Destination upptagen
    CheckDestination --> CheckMoveInList: Destination tom

    CheckMoveInList --> ValidationFailed: Drag inte i valid moves
    CheckMoveInList --> CheckCapture: Drag är giltigt

    CheckCapture --> CheckForcedCapture: Är det ett capture?

    CheckForcedCapture --> ValidationFailed: ForcedCapture ON, icke-capture när capture finns
    CheckForcedCapture --> ValidationSuccess: Inga forced captures, eller capture gjort

    ValidationFailed --> [*]: Returnera false

    ValidationSuccess --> ExecuteMove: Returnera true

    ExecuteMove --> HandleCapture: Validering klar
    HandleCapture --> UpdateBoard
    UpdateBoard --> RecordHistory
    RecordHistory --> [*]: Drag slutfört

    note right of CheckForcedCapture
        Om ForcedCaptures = true:
        - Kolla om några captures finns
        - Tvinga spelare att ta om möjligt
    end note

    note right of HandleCapture
        Om IsCapture(from, to):
        - GetCapturedPosition()
        - RemovePiece()
        - Returnera captured piece
    end note
```

---

## 4. Multi-Jump Capture State Flow

```mermaid
stateDiagram-v2
    [*] --> NormalMove: Spelaren gör drag

    NormalMove --> IsCapture: Evaluera drag

    IsCapture --> RegularMove: Inte capture
    IsCapture --> CaptureMove: Capture!

    RegularMove --> EndTurn: Byt tur
    EndTurn --> [*]

    CaptureMove --> CheckMultiJumpRule: Capture utfört

    CheckMultiJumpRule --> EndTurn: AllowMultipleJumps = false
    CheckMultiJumpRule --> CheckForMoreCaptures: AllowMultipleJumps = true

    CheckForMoreCaptures --> EndTurn: Inga fler captures möjliga
    CheckForMoreCaptures --> MultiJumpMode: Fler captures finns!

    MultiJumpMode --> MultiJumpMode: Spelare MÅSTE fortsätta ta
    MultiJumpMode --> EndTurn: Inga fler captures
    MultiJumpMode --> ManualEndTurn: EndTurn() anropat (om ForcedCaptures = false)

    ManualEndTurn --> EndTurn

    note right of MultiJumpMode
        State:
        - _isInMultiJump = true
        - isInMultiJumpMode = true (UI)
        - Samma pjäs auto-selekterad
        - Bara capture-moves visas
    end note

    note right of CheckForMoreCaptures
        CanPieceCaptureAgain():
        - Hämta validMoves för pjäs
        - Filtrera bara captures
        - Kontrollera om några är giltiga
    end note
```

---

## 5. Screen Navigation State Machine

```mermaid
stateDiagram-v2
    [*] --> MainMenu: Programstart

    MainMenu --> GameScreen: "Start Game" knapp
    MainMenu --> ReplayScreen: "Replay Games" knapp
    MainMenu --> [*]: "Exit Game" knapp

    GameScreen --> GameScreen: Spela damspel
    GameScreen --> MainMenu: "Main Menu" knapp
    GameScreen --> MainMenu: Victory screen "OK" knapp

    ReplayScreen --> ReplayScreen: Titta på replay
    ReplayScreen --> MainMenu: "Main Menu" knapp

    note right of MainMenu
        - Visar logotyp
        - 3 knappar
        - Ingen spellogik
    end note

    note right of GameScreen
        States inom GameScreen:
        - WaitingToStart
        - InProgress
        - Completed
    end note

    note right of ReplayScreen
        - Laddar sparade spel
        - Spelar upp moves
        - Ingen interaktion
    end note
```

---

## 6. Piece Lifecycle State Machine

```mermaid
stateDiagram-v2
    [*] --> Created: Board.Initialize()

    Created --> OnBoard: PlacePiece()

    OnBoard --> Moving: MovePiece() anropat
    Moving --> OnBoard: Flyttad till ny position

    OnBoard --> Captured: RemovePiece() (capture)
    Captured --> [*]: Pjäs borttagen från spelet

    OnBoard --> PromotingToKing: Når motståndarens basrad
    PromotingToKing --> Removed: Gammal pjäs tas bort
    Removed --> KingCreated: Ny KingPiece skapas
    KingCreated --> OnBoard: KingPiece placerad

    note right of Created
        Type: RegularPiece
        - PieceColor satt
        - Position satt
        - IsKing = false
    end note

    note right of OnBoard
        Tillgängliga operationer:
        - GetValidMoves()
        - MovePiece()
        - Kan captures
        - Kan promotas
    end note

    note right of KingCreated
        Type: KingPiece
        - Samma Color
        - Samma Position
        - IsKing = true
        - Kan röra sig backward
    end note

    note right of Captured
        Sparas i:
        - Move.CapturedPiece
        - Kan återställas vid Undo
    end note
```

---

## 7. UI Button State Machine

```mermaid
stateDiagram-v2
    [*] --> Enabled: Button skapas

    Enabled --> Hover: Musen över button
    Hover --> Enabled: Musen lämnar
    Hover --> Pressed: Musknapp nedtryckt
    Pressed --> Hover: Musknapp släppt (innanför)
    Pressed --> Enabled: Musknapp släppt (utanför)

    Hover --> Clicked: MouseReleased event
    Clicked --> Enabled: Event hanterat

    Enabled --> Disabled: Enabled = false
    Disabled --> Enabled: Enabled = true

    Enabled --> Hidden: IsVisible = false
    Hidden --> Enabled: IsVisible = true

    note right of Enabled
        - Normal texture
        - EnabledColor tint
        - Kan interageras med
    end note

    note right of Hover
        - Hover texture
        - EnabledColor tint
        - Musmarkör ändras
    end note

    note right of Pressed
        - Pressed texture
        - EnabledColor tint
        - Visuell feedback
    end note

    note right of Disabled
        - Normal texture
        - DisabledColor tint (grå)
        - Ignorerar input
    end note

    note right of Clicked
        - Clicked event fires
        - EventHandler anropad
        - Action utförs
    end note
```

---

## Key State Transitions

###  Kritiska övergångar:

1. **WaitingToStart → InProgress**: När StartGame() anropas
2. **InProgress → Completed**: När CheckWinner() hittar vinnare
3. **NormalMove → MultiJumpMode**: När capture möjliggör fler captures
4. **MultiJumpMode → EndTurn**: När inga fler captures finns

###  Loopar:

1. **InProgress → InProgress**: Normala drag utan vinnare
2. **PieceSelected → PieceSelected**: Byta vald pjäs
3. **MultiJumpMode → MultiJumpMode**: Fortsätta ta pjäser

### ️ Villkorade övergångar:

1. **CaptureMove → MultiJumpMode**: Kräver `AllowMultipleJumps = true`
2. **MultiJumpMode → ManualEndTurn**: Kräver `ForcedCaptures = false`
3. **CheckForcedCapture**: Blockerar icke-captures om captures finns

---

## State Invariants

### GameStatus.InProgress invarianter:
-   Board är initierat med pjäser
-   Exakt en currentPlayer
-   MoveValidator är satt
-   GameHistory existerar
-   Minst en spelare har giltiga drag

### MultiJump invarianter:
-   `_isInMultiJump = true`
-   `AllowMultipleJumps = true` i RuleSet
-   Senaste draget var ett capture
-   Samma pjäs kan ta igen
-   `currentPlayer` är oförändrad

### Completed invarianter:
-   En spelare har vunnit
-   Antingen: motståndaren har 0 pjäser
-   Eller: motståndaren har inga giltiga drag
-   Inga fler drag tillåtna
-   Victory screen visas
