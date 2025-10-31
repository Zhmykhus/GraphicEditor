using graphic_editor.Helpers;
using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICommand = graphic_editor.Interfaces.ICommand;

namespace graphic_editor.Logic
{
    internal class TextCommand : ICommand
    {
        private Canvas _canvas;
        private TextBox _textBox;
        private System.Drawing.Point _position;
        private Func<bool> _getIsMoveMode;
        private MoveManager _moveManager;

        public TextCommand(Canvas canvas, System.Drawing.Point position, string text, Brush color,
                         double fontSize, Func<bool> getIsMoveMode, MoveManager moveManager)
        {
            _canvas = canvas;
            _position = position;
            _getIsMoveMode = getIsMoveMode;
            _moveManager = moveManager;

            _textBox = new TextBox
            {
                Text = text,
                Foreground = color,
                Width = 200,
                Height = 30,
                FontSize = fontSize,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Arrow,
                IsReadOnly = false 
            };

            Canvas.SetLeft(_textBox, position.X);
            Canvas.SetTop(_textBox, position.Y);

            _textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
            _textBox.MouseMove += TextBox_MouseMove;
            _textBox.MouseUp += TextBox_MouseUp;
            _textBox.LostFocus += TextBox_LostFocus;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_getIsMoveMode()) 
            {
                var textBox = (TextBox)sender;
                var position = e.GetPosition(_canvas);

                textBox.IsReadOnly = true;
                textBox.Focusable = false;

                _moveManager.StartMove(textBox, position);
                e.Handled = true; 
            }
            else
            {
                var textBox = (TextBox)sender;
                textBox.IsReadOnly = false;
                textBox.Focusable = true;
            }
        }

        private void TextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_getIsMoveMode() && e.LeftButton == MouseButtonState.Pressed)
            {
                var textBox = (TextBox)sender;
                var position = e.GetPosition(_canvas);
                _moveManager.DuringMove(position);
            }
        }

        private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_getIsMoveMode())
            {
                var textBox = (TextBox)sender;
                var position = e.GetPosition(_canvas);
                _moveManager.EndMove(position);

                textBox.IsReadOnly = false;
                textBox.Focusable = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.IsReadOnly = false;
            textBox.Focusable = true;
        }

        public void Execute() => _canvas.Children.Add(_textBox);

        public void Undo()
        {
            _textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
            _textBox.MouseMove -= TextBox_MouseMove;
            _textBox.MouseUp -= TextBox_MouseUp;
            _textBox.LostFocus -= TextBox_LostFocus;
            _canvas.Children.Remove(_textBox);
        }
    }
}