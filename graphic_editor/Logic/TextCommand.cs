using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace graphic_editor.Logic
{
    class TextCommand : ICommand
    {
        private Canvas _canvas;
        private TextBox _textBox;

        public TextCommand(Canvas canvas, Point position, string text, Brush color, double fontSize)
        {
            _canvas = canvas;
            _textBox = new TextBox
            {
                Text = text,
                Foreground = color,
                Width = 200,
                Height = 30,
                FontSize = fontSize
            };
            Canvas.SetLeft(_textBox, position.X);
            Canvas.SetTop(_textBox, position.Y);
        }

        public void Execute() => _canvas.Children.Add(_textBox);
        public void Undo() => _canvas.Children.Remove(_textBox);
    }
}
