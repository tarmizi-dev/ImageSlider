using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ImageSlider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // How far (in pixels) a finger must move sideways to count as a swipe.
        private const double SwipeDistance = 60;
        private static readonly TimeSpan ArrowsVisibleTime = TimeSpan.FromSeconds(3);
        private static readonly Duration ArrowsFadeTime = new Duration(TimeSpan.FromMilliseconds(250));
        private static readonly Thickness SelectedCardBorder = new Thickness(4);
        private static readonly Thickness NormalCardBorder = new Thickness(0);

        private readonly DispatcherTimer hideArrowsTimer;
        private int currentIndex;
        private bool arrowsVisible;
        private bool arrowsWereVisibleAtPress;
        private int? swipeTouchId;
        private Point swipeStart;

        public MainWindow()
        {
            InitializeComponent();

            hideArrowsTimer = new DispatcherTimer { Interval = ArrowsVisibleTime };
            hideArrowsTimer.Tick += HideArrowsTimer_Tick;
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            double value = 0.4;
            this.Left = 0;
            this.Top = 0;
            //this.Height = Height * value;
            //this.Width = Width * value;

            //MainCanvas.Height = MainCanvas.Height * value;
            //MainCanvas.Width = MainCanvas.Width * value;

            //gdMainWindow.Height = gdMainWindow.Height * value;
            //gdMainWindow.Width = gdMainWindow.Width * value;

            ShowSlide(0);
            Focus(); // make sure Left/Right key presses reach this window
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // A running timer would keep this window alive in memory.
            hideArrowsTimer.Stop();
            hideArrowsTimer.Tick -= HideArrowsTimer_Tick;
        }

        // ---------- Slides ----------

        private void ShowSlide(int index)
        {
            List<Border> cards = spIMageList.Children.OfType<Border>().ToList();
            if (cards.Count == 0)
            {
                return;
            }

            // Wrap around: after the last card comes the first, and before the first comes the last.
            currentIndex = ((index % cards.Count) + cards.Count) % cards.Count;

            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].BorderBrush = Brushes.White;
                cards[i].BorderThickness = i == currentIndex ? SelectedCardBorder : NormalCardBorder;
            }

            Border selected = cards[currentIndex];

            // Keep the selected card in the middle of the strip.
            double cardCentre = selected
                .TransformToAncestor(spIMageList)
                .Transform(new Point(selected.ActualWidth / 2, 0)).X;
            svImageList.ScrollToHorizontalOffset(cardCentre - svImageList.ViewportWidth / 2);

            // Show the selected card's picture as the big image (once cards have pictures).
            Grid cardGrid = selected.Child as Grid;
            Image cardImage = cardGrid?.Children.OfType<Image>().FirstOrDefault();
            ImageName.Source = cardImage?.Source;
        }

        // ---------- Keyboard ----------

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    ShowSlide(currentIndex - 1);
                    e.Handled = true; // stop the ScrollViewer from also scrolling
                    break;
                case Key.Right:
                    ShowSlide(currentIndex + 1);
                    e.Handled = true;
                    break;
            }
        }

        // ---------- Touch ----------

        private void MainWindow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // The first tap only reveals the arrows; a tap while they are showing presses them.
            arrowsWereVisibleAtPress = arrowsVisible;
            ShowArrows();

            swipeTouchId = e.TouchDevice.Id;
            swipeStart = e.GetTouchPoint(this).Position;
        }

        private void MainWindow_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (swipeTouchId != e.TouchDevice.Id)
            {
                return;
            }
            swipeTouchId = null;

            Vector move = e.GetTouchPoint(this).Position - swipeStart;
            bool isSideways = Math.Abs(move.X) > Math.Abs(move.Y);
            if (isSideways && Math.Abs(move.X) >= SwipeDistance)
            {
                // Swipe left = next slide, swipe right = previous slide.
                ShowSlide(move.X < 0 ? currentIndex + 1 : currentIndex - 1);
                arrowsWereVisibleAtPress = false; // a swipe that ends on an arrow is not also a tap
            }
        }

        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Touch also raises mouse events; those are already handled in PreviewTouchDown.
            if (e.StylusDevice != null)
            {
                return;
            }

            // A real mouse click on an arrow works straight away.
            arrowsWereVisibleAtPress = true;
            ShowArrows();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (arrowsWereVisibleAtPress)
            {
                ShowSlide(currentIndex - 1);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (arrowsWereVisibleAtPress)
            {
                ShowSlide(currentIndex + 1);
            }
        }

        // ---------- Arrow visibility ----------

        private void ShowArrows()
        {
            if (!arrowsVisible)
            {
                arrowsVisible = true;
                gdArrows.BeginAnimation(OpacityProperty, new DoubleAnimation(1, ArrowsFadeTime));
            }

            // Restart the countdown so the arrows stay while the screen is being used.
            hideArrowsTimer.Stop();
            hideArrowsTimer.Start();
        }

        private void HideArrowsTimer_Tick(object sender, EventArgs e)
        {
            hideArrowsTimer.Stop();
            arrowsVisible = false;
            gdArrows.BeginAnimation(OpacityProperty, new DoubleAnimation(0, ArrowsFadeTime));
        }
    }
}
