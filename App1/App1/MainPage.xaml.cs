using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary;
using ChessLibrary.SquareData;
using ChessLibrary.Pieces;
using Xamarin.Forms;
using System.Diagnostics;

namespace App1
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private GameBoard _board;
        private Square _activeSelection;

        public MainPage()
        {
            InitializeComponent();
            _board = new GameBoard();
            DrawBoard();
        }

        private void DrawBoard()
        {
            foreach (View v in chessGrid.Children)
            {
                Square sq = Square.Parse(v.ClassId);
                Piece p = _board[sq];
                var image = (Image)v;
                image.BackgroundColor = (((int)sq.File + (int)sq.Rank) % 2 == 0) ? Color.FromRgb(181, 136, 99) : Color.FromRgb(240, 217, 181);
                if (p == null)
                {
                    image.Source = "";
                }
                else
                {
                    image.Source = $"{p.Owner}{p.GetType().Name}";
                    image.Rotation = (p.Owner == Player.White) ? 0 : 180;
                }

            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            chessGrid.HeightRequest = width;
            chessGrid.WidthRequest = width;
        }

        protected void SquareTapped(object sender, EventArgs e)
        {
            string squareName = ((Image)sender).ClassId;
            Square sq = Square.Parse(squareName);
            if (sq == _activeSelection)
            {
                DrawBoard();
                return;
            }

            if (_activeSelection == null ||
                !_board.IsValidMove(new Move(_activeSelection, sq, _board.WhoseTurn(), PawnPromotion.Queen)))
            {
                if (_board[sq] == null || _board[sq].Owner != _board.WhoseTurn()) return;
                DrawBoard();
                List<Move> validMoves = ChessUtilities.GetValidMovesOfSourceSquare(sq, _board);
                if (validMoves.Count == 0) return;
                _activeSelection = sq;
                this.FindByName<Image>(sq.ToString()).BackgroundColor = Color.Cyan;
                foreach (Move m in validMoves)
                {
                    this.FindByName<Image>(m.Destination.ToString()).BackgroundColor = Color.DarkCyan;
                }

            }
            else
            {
                _board.MakeMove(new Move(_activeSelection, sq, _board.WhoseTurn(), PawnPromotion.Queen), false);
                var sw = new Stopwatch();
                sw.Start();
                DrawBoard();
                sw.Stop();
                DisplayAlert("DrawBoard Duration", sw.ElapsedMilliseconds.ToString(), "OK");
            }


        }
    }
}
