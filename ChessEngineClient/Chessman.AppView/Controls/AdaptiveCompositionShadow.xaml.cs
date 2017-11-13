using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Chessman.Controls
{
    [ContentProperty(Name = nameof(CastingElement))]
    public sealed partial class AdaptiveCompositionShadow : UserControl
    {
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(AdaptiveCompositionShadow), new PropertyMetadata(9.0));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(AdaptiveCompositionShadow), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(AdaptiveCompositionShadow), new PropertyMetadata(0.0));

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(AdaptiveCompositionShadow), new PropertyMetadata(0.0));

        public static readonly DependencyProperty OffsetZProperty =
            DependencyProperty.Register(nameof(OffsetZ), typeof(double), typeof(AdaptiveCompositionShadow), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(AdaptiveCompositionShadow), new PropertyMetadata(1.0));

        private FrameworkElement castingElement = null;
        private Action setCastingElement = null;
        /// <summary>
        /// The blur radius of the drop shadow.
        /// </summary>
        public double BlurRadius
        {
            get
            {
                return (double)GetValue(BlurRadiusProperty);
            }

            set
            {
                SetValue(BlurRadiusProperty, value);
            }
        }

        /// <summary>
        /// The FrameworkElement that this <see cref="CompositionShadow"/> uses to create the mask for the
        /// underlying <see cref="Windows.UI.Composition.DropShadow"/>.
        /// </summary>
        public FrameworkElement CastingElement
        {
            get
            {
                return castingElement;
            }

            set
            {
                castingElement = value;
                setCastingElement();
            }
        }

        /// <summary>
        /// The color of the drop shadow.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// The x offset of the drop shadow.
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// The y offset of the drop shadow.
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// The z offset of the drop shadow.
        /// </summary>
        public double OffsetZ
        {
            get { return (double)GetValue(OffsetZProperty); }
            set { SetValue(OffsetZProperty, value); }
        }

        /// <summary>
        /// The opacity of the drop shadow.
        /// </summary>
        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set {  SetValue(ShadowOpacityProperty, value); }
        }

        public AdaptiveCompositionShadow()
        {
            this.InitializeComponent();

            // the current shadow implementation is only available to  10.0.14393.0
            // and our current min version is lower, make sure that the app will support that too
            // the shadow will not be displayed
            if (ApiInformation.IsTypePresent("Windows.UI.Composition.DropShadow"))
            {
                CompositionShadow compositionControl = new CompositionShadow();
                Binding blurBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(BlurRadius)) };
                compositionControl.SetBinding(CompositionShadow.BlurRadiusProperty, blurBinding);

                Binding colorBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(Color)) };
                compositionControl.SetBinding(CompositionShadow.ColorProperty, colorBinding);

                Binding offsetXBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(OffsetX)) };
                compositionControl.SetBinding(CompositionShadow.OffsetXProperty, offsetXBinding);

                Binding offsetYBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(OffsetY)) };
                compositionControl.SetBinding(CompositionShadow.OffsetYProperty, offsetYBinding);

                Binding offsetZBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(OffsetZ)) };
                compositionControl.SetBinding(CompositionShadow.OffsetZProperty, offsetZBinding);

                Binding shadowOpacityBinding = new Binding() { Source = this, Path = new PropertyPath(nameof(ShadowOpacity)) };
                compositionControl.SetBinding(CompositionShadow.ShadowOpacityProperty, shadowOpacityBinding);

                presenter.Content = compositionControl;
                setCastingElement = () => compositionControl.CastingElement = this.castingElement;
            }
            else
            {
                setCastingElement = () => presenter.Content = CastingElement;
            }
        }
    }
}
