﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OxyPlot.Wpf
{
    /// <summary>
    /// Shows a slider with horizontal and vertical lines, and a templated content control.
    /// The ContentTemplate must be set to define how the slider label looks.
    /// The PlotControls contains a default template in the Themes/Generic.xaml file. 
    /// </summary>
    public class Slider : Canvas
    {
        private readonly Line sliderLine1 = new Line();
        private readonly Line sliderLine2 = new Line();
        private readonly ContentControl content = new ContentControl();
        private readonly PlotControl pc;

        /// <summary>
        /// Gets or sets the slider label format.
        /// </summary>
        /// <value>The slider label format.</value>
        public string SliderLabelFormat { get; set; }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        public DataTemplate ContentTemplate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slider"/> class.
        /// </summary>
        /// <param name="pc">The parent PlotControl.</param>
        public Slider(PlotControl pc)
        {
            SliderLabelFormat = null; // "{0:0.###} {1:0.###}";

            this.pc = pc;
            Children.Add(sliderLine1);
            Children.Add(sliderLine2);
            Children.Add(content);

            sliderLine1.StrokeThickness = 1;
            sliderLine2.StrokeThickness = 1;
            sliderLine1.Stroke = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));
            sliderLine2.Stroke = sliderLine1.Stroke;

            sliderLine1.SnapsToDevicePixels = true;
            sliderLine2.SnapsToDevicePixels = true;
            sliderLine1.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            sliderLine2.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            Hide();
        }

        /// <summary>
        /// Sets the position of the slider.
        /// </summary>
        /// <param name="dp">The data point.</param>
        /// <param name="s">The series.</param>
        public void SetPosition(DataPoint dp, OxyPlot.DataSeries s)
        {

            var pt0 = pc.Transform(dp.X, dp.Y, s.XAxis, s.YAxis);
            var pt1 = pc.Transform(dp.X, s.YAxis.ActualMaximum, s.XAxis, s.YAxis);
            var pt2 = pc.Transform(dp.X, s.YAxis.ActualMinimum, s.XAxis, s.YAxis);
            var pt3 = pc.Transform(s.XAxis.ActualMinimum, dp.Y, s.XAxis, s.YAxis);
            var pt4 = pc.Transform(s.XAxis.ActualMaximum, dp.Y, s.XAxis, s.YAxis);

            if (ContentTemplate != null)
            {
                content.Content = new SliderViewModel(s, dp, SliderLabelFormat);
                content.ContentTemplate = ContentTemplate;
                SetLeft(content, pt0.X);
                SetTop(content, pt0.Y);
            }

            sliderLine1.X1 = pt1.X;
            sliderLine1.Y1 = pt1.Y;
            sliderLine1.X2 = pt2.X;
            sliderLine1.Y2 = pt2.Y;

            sliderLine2.X1 = pt3.X;
            sliderLine2.Y1 = pt3.Y;
            sliderLine2.X2 = pt4.X;
            sliderLine2.Y2 = pt4.Y;

            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the slider.
        /// </summary>
        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }
    }

    public class SliderViewModel
    {
        /// <summary>
        /// Gets or sets the position of the slider (data coordinates).
        /// </summary>
        /// <value>The point.</value>
        public DataPoint Point { get; set; }

        /// <summary>
        /// Gets or sets the data series.
        /// </summary>
        /// <value>The series.</value>
        public OxyPlot.DataSeries Series { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// {0}: {1:0.#####}, {2}: {3:0.#####}
        /// xAxisTitle: xValue, yAxisTitle: yValue
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }

        private const string DEFAULT_FORMAT = "{0}: {1:0.#####}, {2}: {3:0.#####}";

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderViewModel"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        /// <param name="fmt">The format string.</param>
        public SliderViewModel(OxyPlot.DataSeries series, DataPoint point, string fmt = null)
        {
            if (String.IsNullOrEmpty(fmt))
                fmt = DEFAULT_FORMAT;

            this.Format = fmt;
            this.Series = series;
            this.Point = point;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, Format, Series.XAxis.Title, Point.X, Series.YAxis.Title, Point.Y);
        }
    }
}