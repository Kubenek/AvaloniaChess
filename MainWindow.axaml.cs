using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Runtime.Serialization;

using System;
using System.Collections.Generic;
using Avalonia.Remote.Protocol.Input;
using System.Data.SqlTypes;
using System.Net;
using System.Linq;

using Chess.UI;
using Chess.GameManager;
using Chess.Pieces;
using Avalonia.Markup.Xaml.Styling;

namespace Chess;

public class MoveEntry
{
    public string move   { get; set; }
    public string player { get; set; }

    public MoveEntry(string _move, string _player)
    {
        move   = _move;
        player = _player;
    }
}

public partial class MainWindow : Window
{
    private ChessManager    _manager;
    private MoveHighlighter _highlighter;
    private PieceRender     _render;

    private void LogMove(Piece piece, int fromCol, int toRow, int toCol, bool capture, bool isCheck, bool isMate)
    {
        char column = (char)('a' + toCol);
        char fromColumn = (char)('a' + fromCol);

        string ch = isMate ? "#" : (isCheck ? "+" : "");
        string cap = capture ? "x" : "";
        string player = piece.IsWhite ? "White" : "Black";

        int rank = 8 - toRow;

        string template = $"{cap}{column}{rank}{ch}";
        string move = piece switch
        {
            Pawn => (capture ? $"{fromColumn}x" : "") + $"{column}{rank}{ch}",
            Bishop =>  "B" + template, 
            Rook =>  "R" + template, 
            Queen =>  "Q" + template,
            Knight =>  "N" + template,
            King =>  "K" + template,
            _ => ""
        };

        MoveEntry entry = new(move, player);

        MoveList.Items.Insert(0, entry);
    }

    private void ExecuteMove(Piece piece, TextBlock pieceVis, int row, int col)
    {
        int ogCol = piece.Column;
        bool cap = false;
        bool check = false;
        bool mate = false;

        _render.movePiece(GameBoard, pieceVis, piece, row, col, _manager); //? Moves piece and captures the piece visually  
        cap = _manager.movePiece(piece, row, col);                         //? Moves piece and captures the piece logically
        _highlighter.clearHighlights(GameBoard);
        _highlighter.clearCheck(GameBoard);

        King king = _manager.fetchKing(!piece.IsWhite)!;

        if(_manager.isKingInCheck(king)) { 
            check = true;
            _highlighter.highlightCheck(GameBoard, king.Row, king.Column);

            if(_manager.isCheckmate(king.IsWhite)) {
                mate = true;
                Components.showCheckmate(piece.IsWhite, CheckmateOverlay, CheckmateText);
            }
        } else if(_manager.isStalemate(!piece.IsWhite)) Components.showStalemate(CheckmateOverlay, CheckmateText);
        
        Components.updateTurnText(_manager.whiteTurn, TextWhite, TextBlack);
        LogMove(piece, ogCol, row, col, cap, check, mate);
    }

    private void PieceClicked(Piece piece, TextBlock pieceVis)
    {
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

        _highlighter.highlightPieceMoves(piece, GameBoard, pieceVis, legalMoves);
        _highlighter.highlightCaptures(captures);
    }

    public MainWindow()
    {
        InitializeComponent();

        var _board = BoardRender.renderBoard(GameBoard);
        Components.CreateLabels(TopLabels, LeftLabels, BottomLabels, RightLabels);

        _manager = new ChessManager();
        _manager.initializePieces();

        _highlighter = new MoveHighlighter();
        _highlighter.MoveMade += ExecuteMove;

        _render = new PieceRender();
        _render.renderPieces(GameBoard, _manager, _board, _highlighter);
        _render.PiecePressed += PieceClicked;

    }

}