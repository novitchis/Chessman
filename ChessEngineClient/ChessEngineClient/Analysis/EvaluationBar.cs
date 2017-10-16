using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ChessEngineClient.Controls
{
    public sealed class EvaluationBar : Control
    {
        public static readonly DependencyProperty EvaluationProperty = DependencyProperty.Register("Evaluation", typeof(string), typeof(EvaluationBar), new PropertyMetadata("0.0", OnEvaluationChangedThunk));
        public static readonly DependencyProperty PerspectiveProperty = DependencyProperty.Register("Perspective", typeof(SideColor), typeof(EvaluationBar), new PropertyMetadata(SideColor.White, OnPerspectiveChangedThunk));

        private Grid evaluationRangePanel = null;
        private Rectangle evaluationIndicator = null;
        private Storyboard storyBoard = null;

        public string Evaluation
        {
            get { return (string)GetValue(EvaluationProperty); }
            set { SetValue(EvaluationProperty, value); }
        }
        public SideColor Perspective
        {
            get { return (SideColor)GetValue(PerspectiveProperty); }
            set { SetValue(PerspectiveProperty, value); }
        }

        public EvaluationBar()
        {
            this.DefaultStyleKey = typeof(EvaluationBar);
            SizeChanged += (o,e) => OnEvaluationChanged();
            storyBoard = new Storyboard();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            evaluationRangePanel = (Grid)this.GetTemplateChild("EvaluationRangePanel");
            evaluationIndicator = (Rectangle)this.GetTemplateChild("EvaluationIndicator");

            // sometimes dependency properties are set before apply template
            OnPerspectiveChanged();
        }

        private static void OnEvaluationChangedThunk(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EvaluationBar)d).OnEvaluationChanged();
        }

        private void OnEvaluationChanged()
        {
            if (evaluationIndicator == null)
                return;

            // set a local value for ScaleY, since after stoping the storyboard it will be reverted to the initial local value
            // wee want to transition smoothly into the new value
            ((ScaleTransform)(evaluationIndicator.RenderTransform)).ScaleY = ((ScaleTransform)(evaluationIndicator.RenderTransform)).ScaleY;

            storyBoard.Stop();
            storyBoard.Children.Clear();

            double newFillPercentage = GetFillPercentage();

            DoubleAnimation heightAnimation = new DoubleAnimation()
            {
                To = newFillPercentage,
                Duration = TimeSpan.FromSeconds(0.4),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
            };

            storyBoard.Children.Add(heightAnimation);
            Storyboard.SetTarget(heightAnimation, evaluationIndicator);
            Storyboard.SetTargetProperty(heightAnimation, "(Rectangle.RenderTransform).(ScaleTransform.ScaleY)");

            storyBoard.Begin();
        }

        private double GetFillPercentage()
        {
            // evaluation is displayed between -4 and 4
            double evaluationValue = 0.0;
            if (!Double.TryParse(Evaluation, out evaluationValue))
            {
                // -M1, M2
                if (Evaluation.StartsWith("-"))
                    evaluationValue = -4;
                else
                    evaluationValue = 4;
            }

            double displayedEvaluation = Math.Min(Math.Max(evaluationValue, -4), 4);
            double fillPercentage = (displayedEvaluation + 4) / 8;

            return Perspective == SideColor.Black ? fillPercentage : 1 - fillPercentage;
        }

        private static void OnPerspectiveChangedThunk(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EvaluationBar)d).OnPerspectiveChanged();
        }

        private void OnPerspectiveChanged()
        {
            if (evaluationIndicator == null || evaluationRangePanel == null)
                return;

            if (Perspective == SideColor.White)
            {
                evaluationIndicator.Fill = new SolidColorBrush(Colors.DimGray);
                evaluationRangePanel.Background = new SolidColorBrush(Colors.White);
            }
            else if (Perspective == SideColor.Black)
            {
                evaluationIndicator.Fill = new SolidColorBrush(Colors.White);
                evaluationRangePanel.Background = new SolidColorBrush(Colors.DimGray);
            }

            OnEvaluationChanged();
        }
    }
}
