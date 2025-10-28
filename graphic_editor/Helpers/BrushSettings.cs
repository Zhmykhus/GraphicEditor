using System.Windows.Media;

namespace graphic_editor.Helpers
{
    public class BrushSettings
    {
        // SRP: отвечает только за хранение настроек кисти
        public Brush Color { get; set; } = Brushes.Black;
        public double Size { get; set; } = 2;

        public event Action<Brush> ColorChanged;
        public event Action<double> SizeChanged;

        public void SetColor(Brush color)
        {
            Color = color;
            ColorChanged?.Invoke(color);
        }

        public void SetSize(double size)
        {
            Size = size;
            SizeChanged?.Invoke(size);
        }

    }
}