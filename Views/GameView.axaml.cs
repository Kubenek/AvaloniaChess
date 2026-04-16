using Avalonia.Controls;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Chess.Logic;
using Chess.Pieces;
using Chess.UI;
using Avalonia.Media;
using System.Linq;
using Chess.Factories;
using Chess.Modules;

namespace Chess.Views;

public partial class GameView : UserControl
{
    private ChessManager    _manager;
    private MoveEngine      _engine;

    private MoveHighlighter _highlighter;
    private PieceRender     _render;

    private TaskCompletionSource<PieceType>? _promotionChoice;

    private bool _isPromoting  = false;
    private bool _inReviewMode = false;
    private bool _disableInput = false;

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
        _disableInput = true;

        MenuConf.setText("Are you sure you want to exit to the menu?");

        MenuConf.ButtonConfirm += (s, e) => { BackToMenu?.Invoke(s, e); };
        MenuConf.ButtonDecline += (s, e) => { MenuConf.Hide(); _disableInput = false; };

        MenuConf.Show();
    }

    private void ExitReviewMode(object? sender, EventArgs e)
    {
        if(_disableInput) return;

        _inReviewMode = false;

        _highlighter.clearHighlights(GameBoard);
        _render.wipeBoard(GameBoard);
        Components.clearArrows(GameBoard);

        if(_manager._state.CurrentState is GameStateType.Checkmate or GameStateType.Stalemate) CheckmateOverlay.Show();

        _render.renderPieces(GameBoard, _manager);
        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
    }

    private void EntryClicked(MoveEntry entry)
    {
        if(_disableInput) return;

        _inReviewMode = true;

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
        char column = (char)('a' + _move.To.Col);
        char fromColumn = (char)('a' + _move.From.Col);

        string ch = (state is GameStateType.Checkmate) ? "#" : (state is GameStateType.Check ? "+" : "");
        string cap = capture ? "x" : "";
        string player = piece.IsWhite ? "White" : "Black";

        int rank = 8 - _move.To.Row;
        int horizontalDist = Math.Abs(_move.From.Col - _move.To.Col);

        string template = $"{cap}{column}{rank}{ch}";
        string move = piece switch
        {
            Pawn => (capture ? $"{fromColumn}x" : "") + $"{column}{rank}{ch}",
            Bishop =>  "B" + template, 
            Rook =>  "R" + template, 
            Queen =>  "Q" + template,
            Knight =>  "N" + template,
            King =>  (horizontalDist < 2) ? "K" + template : ((_move.To.Col > _move.From.Col) ? "O-O" : "O-O-O"),
            _ => ""
        };

        ChessManager clone = _manager.Clone();
        Piece?[,] board = clone._state.Board;

        MoveEntry entry = new(move, player, board, _move);
        MoveList.AddMove(entry);
    }

    private async void PromotePawn(Pawn pawn)
    {
        if(_disableInput) return;

        PromotionDialog.Show(pawn.IsWhite);
        _isPromoting = true;

        _promotionChoice = new TaskCompletionSource<PieceType>();
        PieceType type = await _promotionChoice.Task;

        var (coords, isWhite) = (pawn.Coords, pawn.IsWhite);
        Piece piece = PieceFactory.createPiece(type, isWhite, coords); 

        _manager._state.Board[coords.Row, coords.Col] = piece;
        _render.updatePieceVisual(GameBoard, pawn, piece);

        if(MoveList.getLength() > 0)
        {
            MoveEntry lastMove = MoveList.getLastMove()!;
            string symbol = type switch
            {
                PieceType.Queen  => "Q",
                PieceType.Rook   => "R",
                PieceType.Knight => "N",
                PieceType.Bishop => "B",
                _                => "Q"
            };

            MoveEntry newEntry = new(lastMove.move + $"={symbol}", lastMove.player, lastMove.board, lastMove.Move);
            MoveList.editMove(0, newEntry);
        }

        King eKing = _manager.fetchKing(!pawn.IsWhite)!;
        if(Evaluator.isKingInCheck(eKing, _manager) && !_highlighter.isHighlighted(eKing.Coords))
            _highlighter.highlightCheck(GameBoard, eKing.Coords);

        _isPromoting = false;
        _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

        PromotionDialog.Hide();
        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
    }

    private void ExecuteMove(Piece piece, TextBlock pieceVis, Move move)
    {
        if(_disableInput) return;

        _render.movePiece(GameBoard, pieceVis, piece, move.To, _manager); //? Moves piece and captures the piece visually  
        bool cap = _engine.movePiece(piece, move.To, _manager, false);    //? Moves piece and captures the piece logically
        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);

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
        if(_disableInput) return;
        if(_isPromoting)  return;
        if(_inReviewMode) return;
        _highlighter.clearHighlights(GameBoard);

        if(piece.IsWhite != _manager._state.IsWhiteTurn) return;

        var moves = piece.availableMoves(_manager);

        List<Position> legalMoves = [];
        List<Position> captures   = [];

        foreach(var position in moves)
        {
            Move move = new(piece.Coords, position);
            if(!_engine.isMoveLegal(move, _manager)) continue;

            legalMoves.Add(move.To);
            var target = _manager.fetchPieceAt(move.To);
            if(target != null && target.IsWhite != piece.IsWhite) captures.Add(move.To);
        }

        if(piece is Pawn pawn)
        {
            for(int i=0; i<2; i++)
            {
                int column = (i==0) ? pawn.Coords.Col - 1 : pawn.Coords.Row + 1;
                int row = pawn.Coords.Row;

                if(column < 0 || column >= 8) continue;
                
                var target = _manager.fetchPieceAt(new Position(row, column));

                if(target != null && target.IsWhite != piece.IsWhite && target is Pawn epawn && epawn.lastMoveDouble)
                {
                    int r = pawn.IsWhite ? pawn.Coords.Row - 1 : pawn.Coords.Row + 1;
                    int c = column;
                    Position posFrom = new(row, pawn.Coords.Col);
                    Position posTo = new(r, c);
                    Move mv = new(posFrom, posTo);

                    if(!_engine.isMoveLegal(mv, _manager)) continue;       
                    legalMoves.Add(posTo); captures.Add(posTo);
                }
                
            }
        }

        if(piece is King king && !king.hasMoved)
        {
            int row = king.Coords.Row;
            int col = king.Coords.Col;

            var c1 = _manager.fetchPieceAt(new Position(row, col+1));
            var c2 = _manager.fetchPieceAt(new Position(row, col+2));
            var c3 = _manager.fetchPieceAt(new Position(row, col+3));

            if(c1 is null && c2 is null && c3 is Rook rook && !rook.hasMoved) 
                legalMoves.Add(new Position(row, col+2));
            
            var cl1 = _manager.fetchPieceAt(new Position(row, col-1));
            var cl2 = _manager.fetchPieceAt(new Position(row, col-2));
            var cl3 = _manager.fetchPieceAt(new Position(row, col-3));
            var cl4 = _manager.fetchPieceAt(new Position(row, col-4));

            if(cl1 is null && cl2 is null && cl3 is null && cl4 is Rook r && !r.hasMoved) 
                legalMoves.Add(new Position(row, col-2));

        }

        _highlighter.highlightPieceMoves(piece, GameBoard, pieceVis, legalMoves);
        _highlighter.highlightCaptures(captures);
    }

    private void onPromotionChoice(PieceType type)
    {
        _promotionChoice?.SetResult(type);
    }

}