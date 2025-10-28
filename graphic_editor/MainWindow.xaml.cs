using graphic_editor.Helpers;
using graphic_editor.Logic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace graphic_editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic.CommandManager _commandManager = new Logic.CommandManager();
        private Point _startPoint;
        private Brush _currentColor = Brushes.Black;
        private double _brushSize = 2;
        private BrushSettings _brushSettings = new BrushSettings();
        public MainWindow()
        {
            InitializeComponent();
            InitializeBrushSettings();
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem is ComboBoxItem item)
            {
                var newColor = item.Background;
                var oldColor = _brushSettings.Color;

                // Создаем команду изменения цвета
                var colorCommand = new ChangeColorCommand(
                    newColor,
                    oldColor,
                    _brushSettings.SetColor
                );
                _commandManager.ExecuteCommand(colorCommand);
            }
        }
        private void InitializeBrushSettings()
        {
            // Подписываемся на изменения настроек
            _brushSettings.ColorChanged += color => UpdateStatus($"Color changed to {color}");
            _brushSettings.SizeChanged += size => UpdateStatus($"Brush size changed to {size}");

            // Устанавливаем начальные значения в ComboBox
            ColorComboBox.SelectedIndex = 0;
            SizeComboBox.SelectedIndex = 1; // размер 2 по умолчанию
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            var position = new System.Drawing.Point(50, 50);
            var textCommand = new TextCommand(DrawingCanvas, position, "Hello World", _currentColor, 14);
            _commandManager.ExecuteCommand(textCommand);
        }

        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Undo();
            UpdateButtons();
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Redo();
            UpdateButtons();
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var endPoint = e.GetPosition(DrawingCanvas);

            // Создаем команду и выполняем через CommandManager
            var drawCommand = new DrawCommand(DrawingCanvas, _startPoint, endPoint, _brushSettings.Color, _brushSettings.Size);
            _commandManager.ExecuteCommand(drawCommand);
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(DrawingCanvas);
        }
        private void UpdateButtons()
        {
            UndoButton.IsEnabled = _commandManager.CanUndo;
            RedoButton.IsEnabled = _commandManager.CanRedo;
        }

        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }

        
    }
}