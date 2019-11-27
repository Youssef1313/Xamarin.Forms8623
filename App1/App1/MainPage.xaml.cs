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
        private List<Image> _colored;

        public MainPage()
        {
            InitializeComponent();
            _board = new GameBoard();
            _colored = new List<Image>();
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

        private void ClearColored()
        {
            foreach (Image colored in _colored)
            {
                Square coloredSquare = Square.Parse(colored.ClassId);
                colored.BackgroundColor = (((int)coloredSquare.File + (int)coloredSquare.Rank) % 2 == 0) ? Color.FromRgb(181, 136, 99) : Color.FromRgb(240, 217, 181);
            }
            _colored = new List<Image>();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var min = (width < height) ? width : height;
            chessGrid.HeightRequest = min;
            chessGrid.WidthRequest = min;
        }

        protected async void SquareTapped(object sender, EventArgs e)
        {
            var clickedImage = (Image)sender;
            Square sq = Square.Parse(clickedImage.ClassId);
            if (sq.Equals(_activeSelection))
            {
                ClearColored();
                _activeSelection = null;
                return;
            }

            if (_activeSelection == null ||
                !_board.IsValidMove(new Move(_activeSelection, sq, _board.WhoseTurn(), PawnPromotion.Queen)))
            {
                ClearColored();
                if (_board[sq] == null || _board[sq].Owner != _board.WhoseTurn()) return;

                List<Move> validMoves = ChessUtilities.GetValidMovesOfSourceSquare(sq, _board);
                if (validMoves.Count == 0) return;
                _activeSelection = sq;
                Image src = this.FindByName<Image>(sq.ToString());
                src.BackgroundColor = Color.DarkCyan;
                _colored.Add(src);
                foreach (Move m in validMoves)
                {
                    Image dest = this.FindByName<Image>(m.Destination.ToString());
                    dest.BackgroundColor = Color.Cyan;
                    _colored.Add(dest);
                }

            }
            else
            {
                _board.MakeMove(new Move(_activeSelection, sq, _board.WhoseTurn(), PawnPromotion.Queen), true);

                Image src = this.FindByName<Image>(_activeSelection.ToString());
                

                Piece p1 = _board[sq];
                Piece p2 = _board[_activeSelection];

                if (p1 == null)
                {
                    clickedImage.Source = "";
                }
                else
                {
                    clickedImage.Source = $"{p1.Owner}{p1.GetType().Name}";
                    clickedImage.Rotation = (p1.Owner == Player.White) ? 0 : 180;
                }

                if (p2 == null)
                {
                    src.Source = "";
                }
                else
                {
                    src.Source = $"{p2.Owner}{p2.GetType().Name}";
                    src.Rotation = (p2.Owner == Player.White) ? 0 : 180;
                }
                ClearColored();
                _activeSelection = null;

                GameState state = _board.GameState;
                if (state == GameState.BlackWinner)
                {
                    await DisplayAlert("Game over!", "Black has won the game.", "OK");
                }
                else if (state == GameState.WhiteWinner)
                {
                    await DisplayAlert("Game over!", "White has won the game.", "OK");
                }
                else if (state == GameState.Stalemate || state == GameState.Draw)
                {
                    await DisplayAlert("Game over!", "Game is draw.", "OK");
                }

            }


        }
    }
}
