using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Runtime.Serialization;

using Chess.Pieces;
using Chess.Board;
using System;
using System.Collections.Generic;
using Avalonia.Remote.Protocol.Input;
using System.Data.SqlTypes;
using System.Net;
using System.Linq;

namespace Chess;

public partial class MainWindow : Window
{
    private Piece? activePiece = null;
    private List<Control> moveHighlights = new List<Control>();
    private Dictionary<Border, IBrush> originalSquareColors = new Dictionary<Border, IBrush>();
    private Border[,] squares = new Border[8,8];

    private bool isWhiteTurn = true;

    private bool isCastlingK = false;
    private bool isCastlingQ = false;

    public MainWindow()
    {
        InitializeComponent();

        CreateChessBoard();
        InitializePieces();   
    }

    private void CreateChessBoard()
    {
        int size = 8;

        for (int i=0; i<size; i++) { GameBoard.RowDefinitions.Add(new RowDefinition(GridLength.Star)); }
        for (int i=0; i<size; i++) { GameBoard.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); }

        for (int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                var square = new Border
                {
                    Background= (i+j) % 2 == 0 ? Brushes.DimGray : Brushes.Gray
                };
                Grid.SetRow(square, i);
                Grid.SetColumn(square, j);

                squares[i, j] = square;

                GameBoard.Children.Add(square);

            }
        }

    }

    private void InitializePieces()
    {
        var board = new Chess.Board.ChessBoard();
        InitializeSide(board, true);
        InitializeSide(board, false);
        
    }

    private void InitializeSide(ChessBoard board, bool isWhite)
    {

        var pawnRow = isWhite ? 6 : 1;

        for(int col = 0; col < 8; col++)
        {
            var pawn = new Pawn(isWhite){ Row = pawnRow, Column = col };
            CreatePiece(pawn, board);
        }

        var backRow = isWhite ? 7 : 0;

        CreatePiece(new Rook(isWhite) { Row = backRow, Column = 0 }, board);
        CreatePiece(new Knight(isWhite) { Row = backRow, Column = 1 }, board);
        CreatePiece(new Bishop(isWhite) { Row = backRow, Column = 2 }, board);
        CreatePiece(new Queen(isWhite) { Row = backRow, Column = 3 }, board);
        CreatePiece(new King(isWhite) { Row = backRow, Column = 4 }, board);
        CreatePiece(new Bishop(isWhite) { Row = backRow, Column = 5 }, board);
        CreatePiece(new Knight(isWhite) { Row = backRow, Column = 6 }, board);
        CreatePiece(new Rook(isWhite) { Row = backRow, Column = 7 }, board);
    }

    private void DotClicked(Piece piece, int row, int col, ChessBoard board)
    {
        ClearMoveHighlights();

        GameBoard.Children.Remove(piece.pieceVisual);
        board.Pieces.Remove(piece);

        piece.Row = row; piece.Column = col;

        if(piece is Pawn pawn) { if(pawn.doubleMove) { pawn.doubleMove = false; } } 
        else if(piece is King king) { if(!king.hasMoved) { king.hasMoved = true; } }
        else if(piece is Rook rook) { if(!rook.hasMoved) { rook.hasMoved = true; } }

        Grid.SetRow(piece.pieceVisual, row);
        Grid.SetColumn(piece.pieceVisual, col);
        GameBoard.Children.Add(piece.pieceVisual);
        board.Pieces.Add(piece);

        isWhiteTurn = !isWhiteTurn;
        TextWhite.Opacity = isWhiteTurn ? 1 : 0.2;
        TextBlack.Opacity = isWhiteTurn ? 0.2 : 1;

        if(isCastlingK && piece is King k)
        {
            var pc = board.GetPieceAt(row, col + 1);

            if(pc is Rook rook) {
                k.hasMoved = true;
                rook.hasMoved = true;

                GameBoard.Children.Remove(rook.pieceVisual);
                board.Pieces.Remove(rook);

                rook.Row = row; rook.Column = col - 1;

                Grid.SetRow(rook.pieceVisual, row);
                Grid.SetColumn(rook.pieceVisual, col-1);
                GameBoard.Children.Add(rook.pieceVisual);
                board.Pieces.Add(rook);
                
                isCastlingK = false;
            } 
        }
        if(isCastlingQ && piece is King kg)
        {
            var pcs = board.GetPieceAt(row, col - 1);
            if(pcs is Rook rk)
            {
                kg.hasMoved = true;
                rk.hasMoved = true;

                GameBoard.Children.Remove(rk.pieceVisual);
                board.Pieces.Remove(rk);

                rk.Row = row; rk.Column = col + 1;

                Grid.SetRow(rk.pieceVisual, row);
                Grid.SetColumn(rk.pieceVisual, col+1);
                GameBoard.Children.Add(rk.pieceVisual);
                board.Pieces.Add(rk);
                
                isCastlingQ = false;
            }
        } 

        isCastlingK = false;
        isCastlingQ = false;    

        var isMated = isMate(board, !piece.IsWhite);
        if(isMated) showGameOver(piece.IsWhite);

    }

    private void PieceClicked(Piece piece, ChessBoard board)
    {

        if(piece.IsWhite != isWhiteTurn)
        {
            
            var square = squares[piece.Row, piece.Column];
            if(square.Background == Brushes.DarkRed && activePiece != null)
            {
                GameBoard.Children.Remove(piece.pieceVisual);
                GameBoard.Children.Remove(activePiece.pieceVisual);
                board.Pieces.Remove(piece); board.Pieces.Remove(activePiece);

                activePiece.Row = piece.Row; activePiece.Column = piece.Column;

                Grid.SetRow(activePiece.pieceVisual, piece.Row);
                Grid.SetColumn(activePiece.pieceVisual, piece.Column);
                GameBoard.Children.Add(activePiece.pieceVisual);
                board.Pieces.Add(activePiece);

                ClearMoveHighlights();

                isWhiteTurn = !isWhiteTurn;

                TextWhite.Opacity = isWhiteTurn ? 1 : 0.2;
                TextBlack.Opacity = isWhiteTurn ? 0.2 : 1;
            }

            return;
        }

        if(activePiece == piece)
        {
            ClearMoveHighlights();
            activePiece = null;
            isCastlingK = false;
            isCastlingQ = false;
            return;
        }

        var moves = piece.GetLegalMoves(board);

        //* Castling
        if(piece is King king && !king.hasMoved)
        {
            int row = king.Row; int col = king.Column;
            if(board.isEmpty(row, col+1) && board.isEmpty(row, col+2) && board.GetPieceAt(row, col+3) is Rook rook && !rook.hasMoved)
            {
                isCastlingK = true;
                moves.Add((row, col+2));
            } else if(board.isEmpty(row, col-1) && board.isEmpty(row, col-2) && board.isEmpty(row, col-3) && board.GetPieceAt(row, col-4) is Rook rk && !rk.hasMoved)
            {
                isCastlingQ = true;
                moves.Add((row, col-3));
            }
            
        }

        ClearMoveHighlights();

        foreach(var move in moves)
        {

            var legal = isMoveLegal(board, piece, move.Item1, move.Item2);
            if(!legal) continue;

            if(board.GetPieceAt(move.Item1, move.Item2) != null)
            {
                var square = squares[move.Item1, move.Item2];
                if (square != null && square.Background != null)
                {
                    originalSquareColors[square] = square.Background;
                    square.Background = Brushes.DarkRed;
                }
                continue;
            }

            var dot = new TextBlock
            {
                Text="●",
                FontSize=16,
                HorizontalAlignment=Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment=Avalonia.Layout.VerticalAlignment.Center,
                Foreground=Brushes.Black,
                Opacity=0.6
            };

            Grid.SetRow(dot, move.Item1);
            Grid.SetColumn(dot, move.Item2);
            GameBoard.Children.Add(dot);

            moveHighlights.Add(dot);

            dot.PointerPressed += (sender, e) =>
            {
                DotClicked(piece, move.Item1, move.Item2, board);  
            };

        }

        activePiece = piece;

    }

    private void ClearMoveHighlights()
    {
        foreach (var highlight in moveHighlights)
        {
            GameBoard.Children.Remove(highlight);
        }
        moveHighlights.Clear();

        foreach(var kvp in originalSquareColors)
        {
            kvp.Key.Background = kvp.Value;
        }
        originalSquareColors.Clear();
    }

    private void CreatePiece(Piece piece, ChessBoard board)
    {
        var pieceVisual = new TextBlock
        {
            Text = piece.Texture,
            FontSize = 50,
            FontFamily = "Segoe UI Symbol",
            Foreground = piece.IsWhite ? Brushes.White : Brushes.Black,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        pieceVisual.PointerPressed += (sender, e) =>
        {
            PieceClicked(piece, board);
        };
        
        piece.pieceVisual = pieceVisual;

        Grid.SetRow(pieceVisual, piece.Row);
        Grid.SetColumn(pieceVisual, piece.Column);
        GameBoard.Children.Add(pieceVisual);

        board.Pieces.Add(piece);
    }

    private bool isMoveLegal(ChessBoard board, Piece piece, int toRow, int toCol)
    {
        ChessBoard clone = board.Clone();

        Piece? clonedPiece = clone.GetPieceAt(piece.Row, piece.Column);

        if(clonedPiece == null) return false;

        Piece? capturedPiece = clone.GetPieceAt(toRow, toCol);

        if(capturedPiece != null) clone.Pieces.Remove(capturedPiece);

        clonedPiece.Row = toRow; clonedPiece.Column = toCol;

        King king = clone.GetKing(piece.IsWhite)!;
        return !clone.isKingInCheck(king);
    }

    private bool isMate(ChessBoard board, bool isWhite)
    {
        var king = board.GetKing(isWhite)!;
        if(!board.isKingInCheck(king)) return false;

        var pieces = board.Pieces.Where(p => p.IsWhite == isWhite).ToList();
        
        foreach(var p in pieces)
        {
            var moves = p.GetLegalMoves(board);

            foreach(var move in moves)
            {
                if(isMoveLegal(board, p, move.Item1, move.Item2)) return false;
            }

        }

        return true;
    }

    private void showGameOver(bool isWhite)
    {
        string winner = isWhite ? "White" : "Black";
        CheckmateText.Text = $"{winner} wins by Checkmate!";
        CheckmateOverlay.IsVisible = true;
    }

}