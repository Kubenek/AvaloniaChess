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

namespace Chess;

public partial class MainWindow : Window
{
    private ChessManager    _manager;
    private MoveHighlighter _highlighter;
    private PieceRender     _render;

    private void ExecuteMove(Piece piece, TextBlock pieceVis, int row, int col)
    {
        _render.movePiece(GameBoard, pieceVis, row, col, _manager); //? Moves piece and captures the piece visually  
        _manager.movePiece(piece, row, col);                        //? Moves piece and captures the piece logically
        _highlighter.clearHighlights(GameBoard);
        
        Components.updateTurnText(_manager.whiteTurn, TextWhite, TextBlack);
    }

    private void PieceClicked(Piece piece, TextBlock pieceVis)
    {
        _highlighter.clearHighlights(GameBoard);

        if(piece.IsWhite != _manager.whiteTurn) return;

        var moves = piece.availableMoves(_manager);
        List<(int, int)> captures = [];

        foreach(var move in moves)
        {
            int row = move.Item1; int col = move.Item2;

            var target = _manager.fetchPieceAt(row, col);

            if(target is null) continue;
            if(target.IsWhite == piece.IsWhite) continue;

            captures.Add(move);

            //? Get piece at location
            //? Check its color with our piece
            //? if they are different, add it to captures List
        }

        _highlighter.highlightPieceMoves(piece, _manager, GameBoard, pieceVis, moves);
        _highlighter.highlightCaptures(captures);
    }

    public MainWindow()
    {
        InitializeComponent();

        var _board = BoardRender.renderBoard(GameBoard);

        _manager = new ChessManager();
        _manager.initializePieces();

        _highlighter = new MoveHighlighter();
        _highlighter.MoveMade += ExecuteMove;

        _render = new PieceRender();
        _render.renderPieces(GameBoard, _manager, _board, _highlighter);
        _render.PiecePressed += PieceClicked;

    }

}