using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ChessEngineClient.Controls
{
    public sealed class EvaluationBar : Control
    {
        public static readonly DependencyProperty EvaluationProperty = DependencyProperty.Register("Evaluation", typeof(string), typeof(EvaluationBar), new PropertyMetadata("0.0", OnEvaluationChangedThunk));

        private Grid evaluationRangePanel = null;
        private Rectangle evaluationIndicator = null;

        public string Evaluation
        {
            get { return (string)GetValue(EvaluationProperty); }
            set { SetValue(EvaluationProperty, value); }
        }

        public EvaluationBar()
        {
            this.DefaultStyleKey = typeof(EvaluationBar);
            SizeChanged += (o,e) => OnEvaluationChanged();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            evaluationRangePanel = (Grid)this.GetTemplateChild("EvaluationRangePanel");
            evaluationIndicator = (Rectangle)this.GetTemplateChild("EvaluationIndicator");
            OnEvaluationChanged();
        }

        private static void OnEvaluationChangedThunk(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EvaluationBar)d).OnEvaluationChanged();
        }

        private void OnEvaluationChanged()
        {
            if (evaluationIndicator == null)
                return;

            evaluationIndicator.Height = evaluationRangePanel.ActualHeight * GetFillPercentage();
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

            return fillPercentage;
        }
    }
}
