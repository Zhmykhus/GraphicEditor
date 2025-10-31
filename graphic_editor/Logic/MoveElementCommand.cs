using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace graphic_editor.Logic
{
    internal class MoveElementCommand :ICommand
    {
        private UIElement _element;
        private Point _oldPosition;
        private Point _newPosition;
        private Canvas _canvas;

        public MoveElementCommand(UIElement element, Point oldPosition, Point newPosition, Canvas canvas)
        {
            _element = element;
            _oldPosition = oldPosition;
            _newPosition = newPosition;
            _canvas = canvas;
        }

        public void Execute()
        {
            Canvas.SetLeft(_element, _newPosition.X);
            Canvas.SetTop(_element, _newPosition.Y);
        }

        public void Undo()
        {
            Canvas.SetLeft(_element, _oldPosition.X);
            Canvas.SetTop(_element, _oldPosition.Y);
        }
    }
}
