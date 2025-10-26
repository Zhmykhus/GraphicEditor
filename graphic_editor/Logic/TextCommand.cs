using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace graphic_editor.Logic
{
    class TextCommand : ICommand
    {
        private Canvas _canvas;
        private TextBox _textBox;
        private bool _isExecuted = false;

        public TextCommand(Canvas canvas, TextBox textBox)
        {
            _canvas = canvas;
            _textBox = textBox;
        }
        public void Execute()
        {
            if (!_isExecuted)
            {
                _canvas.Children.Add(_textBox);
                _isExecuted = true;
            }
        }

        public void Undo()
        {
            if (_isExecuted)
            {
                _canvas.Children.Remove(_textBox);
                _isExecuted = false;
            }
        }
    }
}
