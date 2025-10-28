using graphic_editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphic_editor.Logic
{
    internal class ChangeBrushSizeCommand : ICommand
    {
        private double _oldSize;
        private double _newSize;
        private Action<double> _setSizeAction;

        // SRP: отвечает только за изменение размера
        public ChangeBrushSizeCommand(double newSize, double oldSize, Action<double> setSizeAction)
        {
            _newSize = newSize;
            _oldSize = oldSize;
            _setSizeAction = setSizeAction;
        }

        public void Execute() => _setSizeAction(_newSize);
        public void Undo() => _setSizeAction(_oldSize);
    }
}
