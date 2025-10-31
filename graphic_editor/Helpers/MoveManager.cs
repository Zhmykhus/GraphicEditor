using graphic_editor.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommandManager = graphic_editor.Logic.CommandManager;

namespace graphic_editor.Helpers
{
    internal class MoveManager
    {

        private UIElement _selectedElement;
        private Point _dragStartPoint;
        private Point _elementStartPosition;
        private Canvas _canvas;
        private CommandManager _commandManager;
        public bool IsMoving => _selectedElement != null;

        public MoveManager(Canvas canvas, CommandManager commandManager)
        {
            _canvas = canvas;
            _commandManager = commandManager;
        }

        public void StartMove(UIElement element, Point mousePosition)
        {
            _selectedElement = element;
            _dragStartPoint = mousePosition;
            _elementStartPosition = new Point(
                Canvas.GetLeft(element),
                Canvas.GetTop(element)
            );

            // Визуальная обратная связь
            element.CaptureMouse();
            element.Opacity = 0.7;

            // Меняем курсор в зависимости от типа элемента
            if (element is TextBox textBox)
            {
                textBox.Cursor = Cursors.SizeAll;
                textBox.IsReadOnly = true;
                textBox.Focusable = false;
            }
            else if (element is Image image)
            {
                image.Cursor = Cursors.SizeAll;
            }
        }
        public void DuringMove(Point mousePosition)
        {
            if (_selectedElement != null)
            {
                var offset = mousePosition - _dragStartPoint;
                var newPosition = new Point(
                    _elementStartPosition.X + offset.X,
                    _elementStartPosition.Y + offset.Y
                );

                Canvas.SetLeft(_selectedElement, newPosition.X);
                Canvas.SetTop(_selectedElement, newPosition.Y);
            }
        }

        public void EndMove(Point mousePosition)
        {
            if (_selectedElement != null)
            {
                var offset = mousePosition - _dragStartPoint;
                var newPosition = new Point(
                    _elementStartPosition.X + offset.X,
                    _elementStartPosition.Y + offset.Y
                );

                // Создаем команду перемещения только если позиция изменилась
                if (newPosition != _elementStartPosition)
                {
                    var moveCommand = new MoveElementCommand(
                        _selectedElement,
                        _elementStartPosition,
                        newPosition,
                        _canvas
                    );
                    _commandManager.ExecuteCommand(moveCommand);
                }

                // Возвращаем нормальный вид
                _selectedElement.ReleaseMouseCapture();
                _selectedElement.Opacity = 1.0;

                if (_selectedElement is TextBox textBox)
                {
                    textBox.Cursor = Cursors.Arrow;
                    textBox.IsReadOnly = false;
                    textBox.Focusable = true;
                }
                else if (_selectedElement is Image image)
                {
                    image.Cursor = Cursors.Arrow;
                }

                _selectedElement = null;
            }
        }

        

    }
}
