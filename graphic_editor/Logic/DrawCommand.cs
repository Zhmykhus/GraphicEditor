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
    public class DrawCommand : ICommand
    {
        private Canvas _canvas;
        private UIElement _element;
        private bool _isExecuted = false;

        public DrawCommand(Canvas canvas, UIElement element)
        {
            _canvas = canvas;
            _element = element;
        }

        public void Execute()
        {
            if (!_isExecuted)
            {
                _canvas.Children.Add(_element);
                _isExecuted = true;
            }
        }
        public void Undo()
        {
            if (_isExecuted)
            {
                _canvas.Children.Remove(_element);
                _isExecuted = false;
            }
        }
    }
}
