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
    }

    private void ExitReviewMode(object? sender, EventArgs e)
    {
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
        _inReviewMode = true;

        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);
        _render.wipeBoard(GameBoard);
        CheckmateOverlay.Hide();
        Components.clearArrows(GameBoard);
    
        ChessManager clone = _manager.Clone();
        clone._state.Board = entry.board;

        _render.renderPieces(GameBoard, clone);
        
        List<(int, int)> moves = [entry.fromPos, entry.toPos];
        if((entry.move.Last() == '+' || entry.move.Last() == '#') ? true : false)
        {
            bool pWhite = (entry.player == "White") ? true : false;
            King king = clone.fetchKing(!pWhite)!;
            _highlighter.highlightCheck(GameBoard, king.Row, king.Column);
        }

        _highlighter.highlightReviewMove(GameBoard, moves);

        Components.drawArrow(entry.fromPos, entry.toPos, Colors.Yellow, GameBoard);
        Components.updateReviewModeText(TextWhite, TextBlack);
        
    }

    private void LogMove(Piece piece, int fromRow, int fromCol, int toRow, int toCol, bool capture, GameStateType state)
    {
        char column = (char)('a' + toCol);
        char fromColumn = (char)('a' + fromCol);

        string ch = (state is GameStateType.Checkmate) ? "#" : (state is GameStateType.Check ? "+" : "");
        string cap = capture ? "x" : "";
        string player = piece.IsWhite ? "White" : "Black";

        int rank = 8 - toRow;
        int horizontalDist = Math.Abs(fromCol - toCol);

        string template = $"{cap}{column}{rank}{ch}";
        string move = piece switch
        {
            Pawn => (capture ? $"{fromColumn}x" : "") + $"{column}{rank}{ch}",
            Bishop =>  "B" + template, 
            Rook =>  "R" + template, 
            Queen =>  "Q" + template,
            Knight =>  "N" + template,
            King =>  (horizontalDist < 2) ? "K" + template : ((toCol > fromCol) ? "O-O" : "O-O-O"),
            _ => ""
        };

        ChessManager clone = _manager.Clone();
        Piece?[,] board = clone._state.Board;

        var from = (fromRow, fromCol);
        var to   = (toRow, toCol);

        MoveEntry entry = new(move, player, board, from, to);
        MoveList.AddMove(entry);
    }

    private async void PromotePawn(Pawn pawn)
    {
        PromotionDialog.Show(pawn.IsWhite);
        _isPromoting = true;

        _promotionChoice = new TaskCompletionSource<PieceType>();
        PieceType type = await _promotionChoice.Task;

        var (row, col, isWhite) = (pawn.Row, pawn.Column, pawn.IsWhite);
        Piece piece = PieceFactory.createPiece(type, isWhite, row, col); 

        _manager._state.Board[row, col] = piece;
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

            MoveEntry newEntry = new(lastMove.move + $"={symbol}", lastMove.player, lastMove.board, lastMove.fromPos, lastMove.toPos);
            MoveList.editMove(0, newEntry);
        }

        King eKing = _manager.fetchKing(!pawn.IsWhite)!;
        if(Evaluator.isKingInCheck(eKing, _manager) && !_highlighter.isHighlighted(eKing.Row, eKing.Column))
            _highlighter.highlightCheck(GameBoard, eKing.Row, eKing.Column);

        _isPromoting = false;
        _manager._state.IsWhiteTurn = !_manager._state.IsWhiteTurn;

        PromotionDialog.Hide();
        Components.updateTurnText(_manager._state.IsWhiteTurn, TextWhite, TextBlack);
    }

    private void ExecuteMove(Piece piece, TextBlock pieceVis, int row, int col)
    {
        int fromCol = piece.Column;
        int fromRow = piece.Row;

        _render.movePiece(GameBoard, pieceVis, piece, row, col, _manager); //? Moves piece and captures the piece visually  
        bool cap = _engine.movePiece(piece, row, col, _manager, false);                    //? Moves piece and captures the piece logically
        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);

        if(piece is King k) k.hasMoved = true;
        if(piece is Rook r) r.hasMoved = true;

        GameStateType state = Evaluator.EvaluateGame(!piece.IsWhite, _engine, _manager);
        _manager._state.CurrentState = state;

        King king = _manager.fetchKing(!piece.IsWhite)!;


        if(state is GameStateType.Check) 
            _highlighter.highlightCheck(GameBoard, king.Row, king.Column);
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
        LogMove(piece, fromRow, fromCol, row, col, cap, state);
    }

    private void PieceClicked(Piece piece, TextBlock pieceVis)
    {
        if(_isPromoting)  return;
        if(_inReviewMode) return;
        _highlighter.clearHighlights(GameBoard);

        if(piece.IsWhite != _manager._state.IsWhiteTurn) return;

        var moves = piece.availableMoves(_manager);

        List<(int, int)> legalMoves = [];
        List<(int, int)> captures   = [];

        foreach(var move in moves)
        {
            if(!_engine.isMoveLegal(piece.Row, piece.Column, move.Item1, move.Item2, _manager)) continue;

            legalMoves.Add(move);
            var target = _manager.fetchPieceAt(move.Item1, move.Item2);
            if(target != null && target.IsWhite != piece.IsWhite) captures.Add(move);
        }

        if(piece is Pawn pawn)
        {
            for(int i=0; i<2; i++)
            {
                int column = (i==0) ? pawn.Column - 1 : pawn.Column + 1;
                int row = pawn.Row;

                if(column < 0 || column >= 8) continue;
                
                var target = _manager.fetchPieceAt(row, column);
                if(target != null && target.IsWhite != piece.IsWhite && target is Pawn epawn && epawn.lastMoveDouble)
                {
                    int r = pawn.IsWhite ? pawn.Row - 1 : pawn.Row + 1;
                    int c = column;

                    if(!_engine.isMoveLegal(row, pawn.Column, r, c, _manager)) continue;       
                    legalMoves.Add((r, c)); captures.Add((r, c));
                }
                
            }
        }

        if(piece is King king && !king.hasMoved)
        {
            int row = king.Row;
            int col = king.Column;

            var c1 = _manager.fetchPieceAt(row, col+1);
            var c2 = _manager.fetchPieceAt(row, col+2);
            var c3 = _manager.fetchPieceAt(row, col+3);

            if(c1 is null && c2 is null && c3 is Rook rook && !rook.hasMoved)
                legalMoves.Add((row, col+2));
            
            var cl1 = _manager.fetchPieceAt(row, col-1);
            var cl2 = _manager.fetchPieceAt(row, col-2);
            var cl3 = _manager.fetchPieceAt(row, col-3);
            var cl4 = _manager.fetchPieceAt(row, col-4);

            if(cl1 is null && cl2 is null && cl3 is null && cl4 is Rook r && !r.hasMoved)
                legalMoves.Add((row, col-2));

        }

        _highlighter.highlightPieceMoves(piece, GameBoard, pieceVis, legalMoves);
        _highlighter.highlightCaptures(captures);
    }

    private void onPromotionChoice(PieceType type)
    {
        _promotionChoice?.SetResult(type);
    }

}