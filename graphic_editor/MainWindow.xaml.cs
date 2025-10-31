using graphic_editor.Helpers;
using graphic_editor.Logic;
using Microsoft.Win32;
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
        private BrushSettings _brushSettings = new BrushSettings();
        private MoveManager _moveManager;
        private Point _startPoint;
        private bool _isDrawingMode = true; 
        private Brush _currentColor = Brushes.Black;
        private double _brushSize = 2;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBrushSettings();
            InitializeMoveManager();
            DataContext = this;
        }

        private void InitializeMoveManager()
        {
            _moveManager = new MoveManager(DrawingCanvas, _commandManager);
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem is ComboBoxItem item)
            {
                var newColor = item.Background;
                var oldColor = _brushSettings.Color;

                var colorCommand = new ChangeColorCommand(
                    newColor,
                    oldColor,
                    _brushSettings.SetColor
                );
                _commandManager.ExecuteCommand(colorCommand);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(DrawingCanvas);

            var element = GetElementAtPosition(position);

            if (element != null && !_isDrawingMode)
            {
                _moveManager.StartMove(element, position);
                e.Handled = true; 
            }
            else if (_isDrawingMode)
            {
                _startPoint = position;
            }
        }
        private UIElement GetElementAtPosition(Point position)
        {
            var hitTest = VisualTreeHelper.HitTest(DrawingCanvas, position);
            if (hitTest != null)
            {
                var element = hitTest.VisualHit as FrameworkElement;
                while (element != null && element != DrawingCanvas)
                {
                    if (element is TextBox || element is Image)
                        return element;

                    element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                }
            }
            return null;
        }

        private void InitializeBrushSettings()
        {
            _brushSettings.ColorChanged += color => UpdateStatus($"Color changed to {color}");
            _brushSettings.SizeChanged += size => UpdateStatus($"Brush size changed to {size}");

            ColorComboBox.SelectedIndex = 0;
            SizeComboBox.SelectedIndex = 1; 
        }
        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            _isDrawingMode = !_isDrawingMode;
            UpdateModeIndicator();
        }

        private void UpdateModeIndicator()
        {
            ModeIndicatorText.Text = _isDrawingMode ? "📝 DRAW Mode" : "↔️ MOVE Mode";
            DrawingCanvas.Cursor = _isDrawingMode ? Cursors.Cross : Cursors.Arrow;
            UpdateStatus(_isDrawingMode ? "Switched to Drawing mode" : "Switched to Move mode");
        }
        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            var position = new System.Drawing.Point(50, 50); 

            var textCommand = new TextCommand(
                DrawingCanvas,
                position,
                "Hello World",
                _brushSettings.Color,
                14,
                () => !_isDrawingMode, 
                _moveManager           
            );
            _commandManager.ExecuteCommand(textCommand);
            UpdateStatus("Text added - you can move it in Move mode");
        }

        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SizeComboBox.SelectedItem is ComboBoxItem item &&
            item.Content != null && double.TryParse(item.Content.ToString(), out double newSize))
            {
                var oldSize = _brushSettings.Size;

                var sizeCommand = new ChangeBrushSizeCommand(
                    newSize,
                    oldSize,
                    _brushSettings.SetSize
                );
                _commandManager.ExecuteCommand(sizeCommand);
            }
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
            if (_isDrawingMode && _startPoint != null)
            {
                var endPoint = e.GetPosition(DrawingCanvas);
                var drawCommand = new DrawCommand(DrawingCanvas, _startPoint, endPoint, _brushSettings.Color, _brushSettings.Size);
                _commandManager.ExecuteCommand(drawCommand);
                _startPoint = new Point(0, 0); 
            }
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

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(DrawingCanvas);

            if (_moveManager.IsMoving)
            {
                _moveManager.DuringMove(position);
                UpdateStatus("Moving element...");
            }
            else if (e.LeftButton == MouseButtonState.Pressed && _isDrawingMode)
            {
                UpdateStatus($"Drawing... X: {position.X:F0}, Y: {position.Y:F0}");
            }
            else
            {
                var element = GetElementAtPosition(position);
                if (element != null && !_isDrawingMode)
                {
                    DrawingCanvas.Cursor = Cursors.SizeAll;
                    if (element is TextBox)
                        UpdateStatus("Click and drag to move text");
                    else if (element is Image)
                        UpdateStatus("Click and drag to move image");
                }
                else
                {
                    DrawingCanvas.Cursor = _isDrawingMode ? Cursors.Cross : Cursors.Arrow;
                    UpdateStatus(_isDrawingMode ? "Drawing mode - click to draw" : "Move mode - click on element to move");
                }
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*",
                Title = "Select an image file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var position = new Point(100, 100); 

                    var imageCommand = new AddImageCommand(
                        DrawingCanvas,
                        position,
                        openFileDialog.FileName,
                        () => !_isDrawingMode,
                        _moveManager
                    );

                    _commandManager.ExecuteCommand(imageCommand);
                    UpdateStatus("Image added - you can move it in Move mode");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatus("Failed to load image");
                }
            }
            else
            {
                UpdateStatus("Image selection canceled");
            }
        }
    }
}