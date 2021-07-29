using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kimera.Controls
{
    /// <summary>
    /// Provides the IconView to ListView.
    /// </summary>
    public class PlainView : ViewBase
    {
        #region ::Variables & Properties::

        /// <summary>
        /// The item container style property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = ItemsControl.ItemContainerStyleProperty.AddOwner
        (
            typeof(PlainView)
        );

        /// <summary>
        /// The item template property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = ItemsControl.ItemTemplateProperty.AddOwner
        (
            typeof(PlainView)
        );

        /// <summary>
        /// The item width property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = WrapPanel.ItemWidthProperty.AddOwner
        (
            typeof(PlainView)
        );

        /// <summary>
        /// The item height property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty = WrapPanel.ItemHeightProperty.AddOwner
        (
            typeof(PlainView)
        );

        /// <summary>
        /// The item container style.
        /// </summary>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }
            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// The item template.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// The item width.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }
            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// The item height.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// The default style key.
        /// </summary>
        protected override object DefaultStyleKey
        {
            get
            {
                return new ComponentResourceKey(GetType(), "PlainViewResourceId");
            }
        }

        #endregion
    }
}
