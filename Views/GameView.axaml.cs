using Avalonia.Controls;
using Avalonia.Media;

using System;
using System.Threading.Tasks;
using System.Linq;

using Chess.Factories;
using Chess.Pieces;
using Chess.Logic;
using Chess.UI;

namespace Chess.Views;

public partial class GameView : UserControl
{
    private ChessManager    _manager;
    private MoveEngine      _engine;

    private MoveHighlighter _highlighter;
    private PieceRender     _render;

    private InputType inputType = InputType.Normal;

    private TaskCompletionSource<PieceType>? _promotionChoice;

    public event EventHandler? BackToMenu;

    public GameView()
    {
        InitializeComponent();
        PromotionDialog._promotionChoice += onPromotionChoice;

        BoardRender.renderBoard(GameBoard);
        Components.CreateLabels(TopLabels, LeftLabels, BottomLabels, RightLabels);

        _manager = new ChessManager(new GameState());
        _manager.initializePieces();
        _manager._state.CurrentState = GameStateType.Playing;

        _engine = new MoveEngine();
        _engine.Promotion += PromotePawn;

        _highlighter = new MoveHighlighter();
        _highlighter.MoveMade += ExecuteMove;

        _render = new PieceRender();
        _render.renderPieces(GameBoard, _manager);
        _render.PiecePressed += PieceClicked;

        MoveList.EntryPressed += EntryClicked;
        Components.ExitPressed += ExitReviewMode;

        ButtonMenu.Click += ExitToMenu;
    }

    private void ExitToMenu(object? s, EventArgs e)
    {
        InputType lastInputType = inputType;
        inputType = InputType.MenuMode;

        MenuConf.setText("Are you sure you want to exit to the menu?");

        MenuConf.ButtonConfirm += (s, e) => { BackToMenu?.Invoke(s, e); };
        MenuConf.ButtonDecline += (s, e) => { MenuConf.Hide(); inputType = lastInputType; };

        MenuConf.Show();
    }

    private void ExitReviewMode(object? sender, EventArgs e)
    {
        if(inputType == InputType.MenuMode) return;
        inputType = InputType.Normal;

        _highlighter.clearHighlights(GameBoard);
        _render.wipeBoard(GameBoard);
        Components.clearArrows(GameBoard);

        if(_manager._state.CurrentState is GameStateType.Checkmate or GameStateType.Stalemate) CheckmateOverlay.Show();

        _render.renderPieces(GameBoard, _manager);
        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
    }

    private void EntryClicked(MoveEntry entry)
    {
        if(!(inputType is InputType.Normal or InputType.ReviewMode)) return;
        inputType = InputType.ReviewMode;

        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);
        _render.wipeBoard(GameBoard);
        CheckmateOverlay.Hide();
        Components.clearArrows(GameBoard);
    
        ChessManager clone = _manager.Clone();
        clone._state.Board = entry.board;

        _render.renderPieces(GameBoard, clone);
        
        if((entry.move.Last() == '+' || entry.move.Last() == '#') ? true : false)
        {
            bool pWhite = (entry.player == "White") ? true : false;
            King king = clone.fetchKing(!pWhite)!;
            _highlighter.highlightCheck(GameBoard, king.Coords);
        }

        _highlighter.highlightReviewMove(GameBoard, entry.Move);

        Components.drawArrow(entry.Move, Colors.Yellow, GameBoard);
        Components.updateReviewModeText(TextWhite, TextBlack);
        
    }

    private void LogMove(Piece piece, Move _move, bool capture, GameStateType state)
    {
        string move = MoveNotation.getNotation(piece, _move, capture, state);
        string player = piece.IsWhite ? "White" : "Black";

        ChessManager clone = _manager.Clone();
        Piece?[,] board = clone._state.Board;

        MoveEntry entry = new(move, player, board, _move);
        MoveList.AddMove(entry);
    }

    private async void PromotePawn(Pawn pawn)
    {
        // Show promotion dialog (UI)
        // Await and get player input (Logic)
        // Promote the piece (Logic)
        // Update visuals, move list component and board (UI)
        // Change player turn, reset input restrictions (Logic)
        // Hide dialog, change player turn text components (UI)

        if(inputType != InputType.Normal) return;
        inputType = InputType.PromotionMode;

        PromotionDialog.Show(pawn.IsWhite);

        _promotionChoice = new TaskCompletionSource<PieceType>();
        PieceType type = await _promotionChoice.Task;

        var (coords, isWhite) = (pawn.Coords, pawn.IsWhite);
        Piece piece = PieceFactory.createPiece(type, isWhite, coords); 
        _manager._state.Board[coords.Row, coords.Col] = piece;

        _render.updatePieceVisual(GameBoard, pawn, piece);
        UpdateMoveList(type);
        HighlightCheckIfNeeded(pawn, GameBoard);

        inputType = InputType.Normal;
        _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

        PromotionDialog.Hide();
        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
    }

    private void UpdateMoveList(PieceType type)
    {
        MoveEntry lastMove = MoveList.getLastMove()!;
        string newNotation = MoveNotation.addPromoteNotation(lastMove.move, type);
        lastMove.board = _manager._state.Board;
        MoveEntry newEntry = new(newNotation, lastMove.player, lastMove.board, lastMove.Move);
        MoveList.editMove(0, newEntry);
    }

    private void HighlightCheckIfNeeded(Pawn pawn, Grid GameBoard)
    {
        King enemyKing = _manager.fetchKing(!pawn.IsWhite)!;
        
        if(Evaluator.isKingInCheck(!pawn.IsWhite, _manager) && !_highlighter.isHighlighted(enemyKing.Coords))
            _highlighter.highlightCheck(GameBoard, enemyKing.Coords);
    }

    private void ExecuteMove(Piece piece, TextBlock pieceVis, Move move)
    {
        if(inputType != InputType.Normal) return;

        _render.movePiece(GameBoard, pieceVis, piece, move.To, _manager); //? Moves piece and captures the piece visually  
        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);

        bool cap = _engine.movePiece(piece, move.To, _manager, false);

        if(piece is King k) k.hasMoved = true;
        if(piece is Rook r) r.hasMoved = true;

        GameStateType state = Evaluator.EvaluateGame(!piece.IsWhite, _engine, _manager);
        _manager._state.CurrentState = state;

        King king = _manager.fetchKing(!piece.IsWhite)!;


        if(state is GameStateType.Check) 
            _highlighter.highlightCheck(GameBoard, king.Coords);
        if(state is GameStateType.Checkmate) {
            string player = piece.IsWhite ? "White" : "Black";
            CheckmateOverlay.setText($"{player} wins by Checkmate!");
            CheckmateOverlay.Show();
        }
        else if(state is GameStateType.Stalemate) {
            CheckmateOverlay.setText("Game ends in Stalemate!");
            CheckmateOverlay.Show();
        }

        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
        LogMove(piece, move, cap, state);
    }

    private void PieceClicked(Piece piece, TextBlock pieceVis)
    {
        if(inputType != InputType.Normal) return;
        _highlighter.clearHighlights(GameBoard);

        if(piece.IsWhite != _manager._state.IsWhiteTurn) return;

        var (moves, captures) = _engine.getPieceMoves(piece, _manager);

        _highlighter.highlightPieceMoves(piece, GameBoard, pieceVis, moves);
        _highlighter.highlightCaptures(captures);
    }

    private void onPromotionChoice(PieceType type)
    {
        if(inputType == InputType.PromotionMode)
            _promotionChoice?.SetResult(type);
    }

}