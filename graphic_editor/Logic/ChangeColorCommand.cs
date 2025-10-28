using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace graphic_editor.Logic
{
    internal class ChangeColorCommand : ICommand
    {
        private Brush _oldColor;
        private Brush _newColor;
        private Action<Brush> _setColorAction;

        // SRP: отвечает только за изменение цвета
        public ChangeColorCommand(Brush newColor, Brush oldColor, Action<Brush> setColorAction)
        {
            _newColor = newColor;
            _oldColor = oldColor;
            _setColorAction = setColorAction;
        }

        public void Execute() => _setColorAction(_newColor);
        public void Undo() => _setColorAction(_oldColor);

    }
}
