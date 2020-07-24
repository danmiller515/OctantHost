using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OctantApp.CustomControls
{
    public partial class MoveBar : UserControl
    {
        public MoveBar()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public Uri ImageUri
        {
            get { return (Uri)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(MoveBar), null);

        private void borMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseClick(e);
        }

        public delegate void ClickEventHandler(object sender, RoutedEventArgs e);
        public event ClickEventHandler Click;

        protected void RaiseClick(RoutedEventArgs e)
        {
            if (null != Click)
                Click(this, e);
        }
    }
}
