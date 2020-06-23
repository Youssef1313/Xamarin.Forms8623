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
using App1.Models;

namespace App1
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private GameBoard _board;
        private Square _activeSelection;
        private List<ChessSquare> _colored;

        public MainPage()
        {
            InitializeComponent();
            _board = new GameBoard();
            _colored = new List<ChessSquare>();

            /*
            for (char c = 'A'; c <= 'H'; c++)
            {
                for (byte i = 1; i <= 8; i++)
                {
                    var chessSquare = new ChessSquare
                    {
                        Square = new Square(c, i)
                    };
                    chessSquare.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SquareTapped(chessSquare)) });
                    chessGrid.Children.Add(chessSquare, c - 'A', '8' - i);
                }
            }
            THIS IS NOT WORKING FOR UNKNOWN REASON. !!
            */
            DrawBoard();
        }

        private void DrawBoard()
        {
            foreach (ChessSquare cq in chessGrid.Children)
            {
                //cq.Square = Square.Parse(cq.ClassId);
                Piece p = _board[cq.Square];
                cq.BackgroundColor = (((int)cq.Square.File + (int)cq.Square.Rank) % 2 == 0) ? Color.FromRgb(181, 136, 99) : Color.FromRgb(240, 217, 181);
                if (p == null)
                {
                    cq.Source = "";
                }
                else
                {
                    cq.Source = $"{p.Owner}{p.GetType().Name}";
                    cq.Rotation = (p.Owner == Player.White) ? 0 : 180;
                }

            }
        }

        private void ClearColored()
        {
            foreach (ChessSquare colored in _colored)
            {
                colored.BackgroundColor = (((int)colored.Square.File + (int)colored.Square.Rank) % 2 == 0) ? Color.FromRgb(181, 136, 99) : Color.FromRgb(240, 217, 181);
            }
            _colored = new List<ChessSquare>();
            _activeSelection = null;
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
           ChessSquare chessSquare = (ChessSquare)sender;
            if (chessSquare.Square.Equals(_activeSelection))
            {
                ClearColored();
                return;
            }

            if (_activeSelection == null ||
                !_board.IsValidMove(new Move(_activeSelection, chessSquare.Square, _board.WhoseTurn(), PawnPromotion.Queen)))
            {
                ClearColored();
                if (_board[chessSquare.Square] == null || _board[chessSquare.Square].Owner != _board.WhoseTurn()) return;

                List<Move> validMoves = ChessUtilities.GetValidMovesOfSourceSquare(chessSquare.Square, _board);
                if (validMoves.Count == 0) return;
                _activeSelection = chessSquare.Square;
                ChessSquare src = this.FindByName<ChessSquare>(chessSquare.Square.ToString());
                src.BackgroundColor = Color.DarkCyan;
                _colored.Add(src);
                foreach (Move m in validMoves)
                {
                    ChessSquare dest = this.FindByName<ChessSquare>(m.Destination.ToString());
                    dest.BackgroundColor = Color.Cyan;
                    _colored.Add(dest);
                }
                

            }
            else
            {
                PawnPromotion? promotion = null;
                if (_board[_activeSelection] is Pawn && (chessSquare.Square.Rank == Rank.First || chessSquare.Square.Rank == Rank.Eighth))
                {
                    string strProm;
                    do
                    {
                        strProm = await DisplayActionSheet("Promote to what ?", null, null, "Queen", "Rook", "Knight", "Bishop");
                    } while (strProm == null);
                    Enum.TryParse(strProm, out PawnPromotion temp_promotion);
                    promotion = temp_promotion;
                }
                var move = new Move(_activeSelection, chessSquare.Square, _board.WhoseTurn(), promotion);
                _board.MakeMove(move, isMoveValidated: true);

                ChessSquare src = this.FindByName<ChessSquare>(_activeSelection.ToString());
                

                Piece p1 = _board[chessSquare.Square];
                Piece p2 = _board[_activeSelection];

                if (p1 is King && move.GetAbsDeltaX() == 2 ||
                    p1 is Pawn && move.GetAbsDeltaX() == 1) // Draw complete board only on Special Moves (Castle, Enpassant).
                {
                    DrawBoard();
                }
                else // On regular moves, draw particular squares only. (For performance reasons)
                {
                    if (p1 == null)
                    {
                        chessSquare.Source = "";
                    }
                    else
                    {
                        chessSquare.Source = $"{p1.Owner}{p1.GetType().Name}";
                        chessSquare.Rotation = (p1.Owner == Player.White) ? 0 : 180;
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
                }

                ClearColored();

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
