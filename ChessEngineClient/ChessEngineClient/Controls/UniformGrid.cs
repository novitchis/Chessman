﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient.Controls
{
    /// <summary>
    /// //TODO: transform this into a chess board panel
    /// Provides a way to arrange content in a grid where all the cells in the grid have the same size. 
    /// </summary>
    public class UniformGrid : Panel
    {
        /// <summary>
        /// Identifies the Columns dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                "Columns",
                typeof(int),
                typeof(UniformGrid),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the Rows dependency property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                "Rows",
                typeof(int),
                typeof(UniformGrid),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the FirstColumn dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstColumnProperty =
            DependencyProperty.Register(
                "FirstColumn",
                typeof(int),
                typeof(UniformGrid),
                new PropertyMetadata(0));

        /// <summary>
        /// Are items squares
        /// </summary>
        public static readonly DependencyProperty SquareItemsProperty =
            DependencyProperty.Register(
                "SquareItems", 
                typeof(bool), 
                typeof(UniformGrid), 
                new PropertyMetadata(false));

        private int _columns;
        private int _rows;

        /// <summary>
        /// Gets or sets the number of columns that are in the grid.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of leading blank cells in the first row of the grid.
        /// </summary>
        public int FirstColumn
        {
            get { return (int)GetValue(FirstColumnProperty); }
            set { SetValue(FirstColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of rows that are in the grid.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the items are squares
        /// </summary>
        public bool SquareItems
        {
            get { return (bool)GetValue(SquareItemsProperty); }
            set { SetValue(SquareItemsProperty, value); }
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override
        /// this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="arrangeSize">
        /// The final area within the parent that this object should use to arrange itself
        /// and its children.
        /// </param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (SquareItems)
            {
                double minSize = Math.Min(arrangeSize.Width, arrangeSize.Height);
                arrangeSize = new Size(minSize, minSize);
            }

            var finalRect = new Rect(0.0, 0.0, arrangeSize.Width / _columns, arrangeSize.Height / _rows);            

            var width = finalRect.Width;
            var num2 = arrangeSize.Width - 1.0;
            finalRect.X += finalRect.Width * this.FirstColumn;

            foreach (var element in Children)
            {
                element.Arrange(finalRect);

                if (element.Visibility != Visibility.Collapsed)
                {
                    finalRect.X += width;

                    if (finalRect.X >= num2)
                    {
                        finalRect.Y += finalRect.Height;
                        finalRect.X = 0.0;
                    }
                }
            }

            return arrangeSize;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes can
        /// override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="constraint">
        /// The available size that this object can give to child objects. Infinity can
        /// be specified as a value to indicate that the object will size to whatever
        /// content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its
        /// calculations of the allocated sizes for child objects or based on other considerations
        /// such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();
            var availableSize = new Size(constraint.Width / (_columns), constraint.Height / (_rows));

            if (SquareItems)
            {
                var minSize = Math.Min(availableSize.Height, availableSize.Width);
                availableSize = new Size(minSize, minSize);
            }

            var width = 0.0;
            var height = 0.0;
            var num3 = 0;
            var count = Children.Count;

            while (num3 < count)
            {
                var element = Children[num3];
                element.Measure(availableSize);
                var desiredSize = element.DesiredSize;

                if (width < desiredSize.Width)
                {
                    width = desiredSize.Width;
                }

                if (height < desiredSize.Height)
                {
                    height = desiredSize.Height;
                }

                num3++;
            }

            return new Size(width * _columns, height * _rows);
        }

        private void UpdateComputedValues()
        {
            _columns = this.Columns;
            _rows = this.Rows;

            if (this.FirstColumn >= _columns)
            {
                this.FirstColumn = 0;
            }

            if ((_rows == 0) || (_columns == 0))
            {
                var num = 0;
                var num2 = 0;

                var count = Children.Count;

                while (num2 < count)
                {
                    var element = Children[num2];

                    if (element.Visibility != Visibility.Collapsed)
                    {
                        num++;
                    }

                    num2++;
                }

                if (num == 0)
                {
                    num = 1;
                }

                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = ((num + this.FirstColumn) + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt(num);

                        if ((_rows * _rows) < num)
                        {
                            _rows++;
                        }

                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (num + (_rows - 1)) / _rows;
                }
            }
        }
    }
}
