using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace graphic_editor.Logic
{
    public class DrawCommand : ICommand
    {
        private Canvas _canvas;
        private Line _line;

        public DrawCommand(Canvas canvas, Point start, Point end, Brush color, double thickness)
        {
            _canvas = canvas;
            _line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = color,
                StrokeThickness = thickness
            };
        }

        public void Execute() => _canvas.Children.Add(_line);
        public void Undo() => _canvas.Children.Remove(_line);


    }
}
