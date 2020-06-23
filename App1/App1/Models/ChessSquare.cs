using ChessSharp.SquareData;
using Xamarin.Forms;

namespace App1.Models
{
    public class ChessSquare : Image
    {
        public Square Square { get; set; }

        //public static readonly BindableProperty SquareProperty =
        //    BindableProperty.Create(nameof(Square), typeof(Square), typeof(ChessSquare), null);

        //public Square Square
        //{
        //    get { return Square.Parse((string)GetValue(SquareProperty)); }
        //    set { SetValue(SquareProperty, value); }
        //}
    }
}
