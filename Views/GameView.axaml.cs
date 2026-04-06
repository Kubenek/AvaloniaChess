using Avalonia.Controls;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Chess.Modules;
using Chess.GameManager;
using Chess.Pieces;
using Chess.UI;

namespace Chess.Views;

public partial class GameView : UserControl
{
    private ChessManager    _manager;
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

        _manager = new ChessManager();
        _manager.initializePieces();
        _manager.Promotion += PromotePawn;

        _highlighter = new MoveHighlighter();
        _highlighter.MoveMade += ExecuteMove;

        _render = new PieceRender();
        _render.renderPieces(GameBoard, _manager);
        _render.PiecePressed += PieceClicked;

        MoveList.EntryPressed += EntryClicked;
    }

    private void EntryClicked(MoveEntry entry)
    {
        var lastMove = MoveList.getLastMove();

        if(entry == lastMove)
        {
            _inReviewMode = false;
            _render.wipeBoard(GameBoard);
            _render.renderPieces(GameBoard, _manager);
            _highlighter.clearHighlights(GameBoard);
        } else
        {
            _inReviewMode = true;
        
            ChessManager clone = _manager.Clone();
            clone.pieces = entry.board;
            
            _render.wipeBoard(GameBoard);
            _render.renderPieces(GameBoard, clone);
            _highlighter.clearHighlights(GameBoard);

            List<(int, int)> moves = [entry.fromPos, entry.toPos];
            _highlighter.highlightReviewMove(GameBoard, moves);
        }  
    }

    private void LogMove(Piece piece, int fromRow, int fromCol, int toRow, int toCol, bool capture, bool isCheck, bool isMate)
    {
        char column = (char)('a' + toCol);
        char fromColumn = (char)('a' + fromCol);

        string ch = isMate ? "#" : (isCheck ? "+" : "");
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
        Piece?[,] board = clone.pieces;

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

        bool isWhite = pawn.IsWhite;

        Piece piece = type switch 
        {
            PieceType.Queen  => new   Queen(isWhite),
            PieceType.Rook   => new    Rook(isWhite),
            PieceType.Knight => new  Knight(isWhite),
            PieceType.Bishop => new  Bishop(isWhite),
            _                => new   Queen(isWhite)
        };

        int row = pawn.Row; int col = pawn.Column;
        _manager.pieces[row, col] = null;

        piece.Row = row; piece.Column = col;

        _manager.pieces[row, col] = piece;
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
        if(_manager.isKingInCheck(eKing) && !_highlighter.isHighlighted(eKing.Row, eKing.Column))
            _highlighter.highlightCheck(GameBoard, eKing.Row, eKing.Column);

        _isPromoting = false;
        _manager.whiteTurn = !_manager.whiteTurn;

        PromotionDialog.Hide();
        Components.updateTurnText(_manager.whiteTurn, TextWhite, TextBlack);
    }

    private (bool isCheck, bool isMate, bool isStalemate) EvaluateGame(bool isWhite)
    {
        King king = _manager.fetchKing(isWhite)
        !;
        bool check     =           _manager.isKingInCheck(king);
        bool mate      =  check && _manager.isCheckmate(isWhite);
        bool stalemate = !check && _manager.isStalemate(isWhite);

        return (check, mate, stalemate);
    }

    private void ExecuteMove(Piece piece, TextBlock pieceVis, int row, int col)
    {
        int fromCol = piece.Column;
        int fromRow = piece.Row;

        _render.movePiece(GameBoard, pieceVis, piece, row, col, _manager); //? Moves piece and captures the piece visually  
        bool cap = _manager.movePiece(piece, row, col);                    //? Moves piece and captures the piece logically
        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);

        if(piece is King k) k.hasMoved = true;
        if(piece is Rook r) r.hasMoved = true;

        var (isCheck, isMate, isStalemate) = EvaluateGame(!piece.IsWhite);
        King king = _manager.fetchKing(!piece.IsWhite)!;


        if(isCheck) 
            _highlighter.highlightCheck(GameBoard, king.Row, king.Column);
        if(isMate) {
            string player = piece.IsWhite ? "White" : "Black";
            CheckmateOverlay.setText($"{player} wins by Checkmate!");
            CheckmateOverlay.Show();
        }
        else if(isStalemate) {
            CheckmateOverlay.setText("Game ends in Stalemate!");
            CheckmateOverlay.Show();
        }

        Components.updateTurnText(_manager.whiteTurn, TextWhite, TextBlack);
        LogMove(piece, fromRow, fromCol, row, col, cap, isCheck, isMate);
    }

    private void PieceClicked(Piece piece, TextBlock pieceVis)
    {
        if(_isPromoting)  return;
        if(_inReviewMode) return;
        _highlighter.clearHighlights(GameBoard);

        if(piece.IsWhite != _manager.whiteTurn) return;

        var moves = piece.availableMoves(_manager);

        List<(int, int)> legalMoves = [];
        List<(int, int)> captures   = [];

        foreach(var move in moves)
        {
            if(!_manager.isMoveLegal(piece.Row, piece.Column, move.Item1, move.Item2)) continue;

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

                    if(!_manager.isMoveLegal(row, pawn.Column, r, c)) continue;       
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