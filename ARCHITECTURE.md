# Architecture

## Overview
Avalonia Chess follows a modular architecture with strict separation between game logic and UI rendering. Logic operates independently of visual presentation.

## Project Structure
```
Chess/
├── GameManager/         # Core game logic and rules
├── Pieces/              # Piece definitions and movement
├── UI/                  # Rendering and visual updates
├── Modules/             # Reusable AXAML components
├── Models/              # Data structures (enums, classes)
├── MainWindow.axaml     # Main application shell
└── MainWindow.axaml.cs. # Main code that glues the project together
```

## Core Components

### GameManager (Logic Layer)
**Purpose:** Game state and rule enforcement

**Responsibilities:**
- Initialize pieces & maintain 2D piece array
- Validate and execute moves
- Detect check, checkmate, stalemate
- Fetch pieces when asked to
- Handle special moves (en passant, promotion)
- Clone for simulation

### Pieces (Domain Layer)
**Purpose:** Chess piece definitions

**Structure:**
- `Piece.cs` - Abstract base class
- Individual piece classes (`Pawn.cs`, `Knight.cs`, etc.)

**Responsibilities:**
- Define movement patterns
- Clone for simulation

### UI (Presentation Layer)
**Purpose:** Visual rendering separate from logic

**Key Classes:**
- `BoardRender.cs`     - Creates visual board grid
- `PieceRender.cs`     - Manages piece visuals and mapping
- `MoveHighlighter.cs` - Shows legal moves and captures

**Pattern:** UI components maintain a mapping between logical pieces and visual TextBlocks

### Modules (Reusable Components)
**Purpose:** Self-contained AXAML components

**Examples:**
- `Promotion.axaml` - Pawn promotion dialog
- `OvCheckmate.axaml` - Game over overlay

**Pattern:** UserControls with events for communication

### Move Notation & History
**Purpose:** Record and display game moves in standard algebraic notation (SAN)

**Responsibilities:**
- Convert moves to algebraic notation (e.g., `e4`, `Nf3`, `exd5`)
- Track captures, checks, and checkmates
- Display move history in UI ListBox

**Notation Rules:**
- Piece moves: `Nf3`, `Qh5`
- Pawn moves: `e4` (just the square)
- Captures: `Nxf3`, `exd5` (pawn captures show file)
- Check: `+` suffix
- Checkmate: `#` suffix

**UI Integration:**
- Two-column display (Move | Player)
- Latest moves appear at top
- Selection highlights current move

## Key Design Patterns

### Separation of Concerns
- **Logic:** GameManager has no UI dependencies
- **UI:** Rendering layer has no game rule knowledge
- **Communication:** Events and method calls

### Move Validation Flow
1. User clicks piece → UI requests valid moves
2. GameManager calculates pseudo-legal moves
3. GameManager filters moves that expose king to check (simulation via board cloning)
4. UI displays highlighted squares
5. User clicks square → GameManager executes move
6. UI updates visuals

### Board State Management
- **Single source of truth:** `ChessManager.pieces[8,8]`
- **Cloning for simulation:** Deep copy for "what-if" scenarios
- **No stored state:** Check/mate calculated on demand

### Event-Driven UI
- Components fire events (e.g., `PieceSelected`, `MoveSelected`)
- MainWindow orchestrates responses
- Async/await for user input (promotion dialog)

## Data Flow
```
User Click
    ↓
UI Layer (captures event)
    ↓
GameManager (validates move)
    ↓
UI Layer (updates visuals)
    ↓
GameManager (checks game state)
    ↓
UI Layer (shows check/mate/stalemate)
```