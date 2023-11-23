using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolygonViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var poly = new Polygon
            {
                Stroke = Brushes.Black,
                Points = new PointCollection(
                    new[] {
                        new Point(0,0),
                        new Point(1, 10),
                        new Point(10,10) })
            };
            canvas.Children.Add(poly);

            pointsText.TextChanged += PointsText_TextChanged;
        }

        private void PointsText_TextChanged(object sender, TextChangedEventArgs e)
        {
            canvas.Children.Clear();
            try
            {
                var points = ParsePoints(pointsText.Text);
                var poly = new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.2,
                    Points = new PointCollection(points)
                };
                canvas.Children.Add(poly);
            }
            catch (ParseException pe)
            {
                status.Text = $"{pe.Message}. Index: {pe.Index}";
            }
        }

        private IEnumerable<Point> ParsePoints(string text)
        {
            var result = new List<Point>();
            var isDone = false;
            var startIndex = 0;
            do
            {
                var posOpenBrace = text.IndexOf('{', startIndex);
                if (posOpenBrace < 0)
                {
                    isDone = true;
                    break;
                }
                var posCloseBrace = text.IndexOf('}', posOpenBrace);
                if (posCloseBrace < 0) break;

                var content = text.Substring(posOpenBrace + 1, posCloseBrace - posOpenBrace - 1);
                try {
                    var point = ParsePoint(content);
                    result.Add(point);
                }
                catch (Exception e)
                {
                    throw new ParseException("Can't parse", e) { Index = posOpenBrace };
                }
                startIndex = posCloseBrace;
                isDone = posCloseBrace == text.Length;
            } while (! isDone);

            var statusText = isDone ? "Ok" : $"Parsed until index {startIndex}";
            status.Text = statusText;
            return result;
        }

        /// <summary>
        /// Parses a point
        /// </summary>
        /// <param name="content">A string on the form 'X=1.23 Y=-9.87'</param>
        /// <returns></returns>
        private Point ParsePoint(string content)
        {
            const string ignoredCharacters = "xXyY=";
            var filtered = new string(content.Where(c => ! ignoredCharacters.Contains(c)).ToArray());
            return Point.Parse(filtered);
        }
    }
}
