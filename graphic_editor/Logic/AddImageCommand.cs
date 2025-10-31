using System;
using System.Windows;
using System.Windows.Controls;        
using System.Windows.Input;      
using System.Windows.Media;         
using System.Windows.Media.Imaging;  
using graphic_editor.Helpers;
using graphic_editor.Interfaces;
using Microsoft.Win32;
using ICommand = graphic_editor.Interfaces.ICommand;

namespace graphic_editor.Logic
{
    internal class AddImageCommand : ICommand
    {
        private Canvas _canvas;
        private Image _image;
        private Point _position;
        private Func<bool> _getIsMoveMode;
        private MoveManager _moveManager;

        public AddImageCommand(Canvas canvas, Point position, string imagePath,
                             Func<bool> getIsMoveMode, MoveManager moveManager)
        {
            _canvas = canvas;
            _position = position;
            _getIsMoveMode = getIsMoveMode;
            _moveManager = moveManager;

            _image = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(imagePath)),
                Width = 100,
                Height = 100,
                Cursor = Cursors.Arrow,
                Stretch = Stretch.Uniform
            };

            Canvas.SetLeft(_image, position.X);
            Canvas.SetTop(_image, position.Y);

            _image.MouseDown += Image_MouseDown;
            _image.MouseMove += Image_MouseMove;
            _image.MouseUp += Image_MouseUp;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_getIsMoveMode())
            {
                var image = ((System.Windows.Controls.Image)sender);
                var position = e.GetPosition(_canvas);
                _moveManager.StartMove(image, position);
                e.Handled = true;
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_getIsMoveMode() && e.LeftButton == MouseButtonState.Pressed)
            {
                var image = (System.Windows.Controls.Image)sender;
                var position = e.GetPosition(_canvas);
                _moveManager.DuringMove(position);
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_getIsMoveMode())
            {
                var image = (System.Windows.Controls.Image)sender;
                var position = e.GetPosition(_canvas);
                _moveManager.EndMove(position);
            }
        }

        public void Execute() => _canvas.Children.Add(_image);

        public void Undo()
        {
            _image.MouseDown -= Image_MouseDown;
            _image.MouseMove -= Image_MouseMove;
            _image.MouseUp -= Image_MouseUp;
            _canvas.Children.Remove(_image);
        }
    }
}

