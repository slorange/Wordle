using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wordle {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

        int N = 5;

        SolidColorBrush Grey = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
        SolidColorBrush Yellow = new SolidColorBrush(Color.FromArgb(255, 220, 190, 20));
        SolidColorBrush Green = new SolidColorBrush(Color.FromArgb(255, 90, 180, 90));

        Dictionary<int, List<Word>> allWords;
        Game game;
        int row = 0;
        string currentWord;

        public MainWindow() {
			InitializeComponent();
            
            allWords = Data.GetWordFrequencies(N);
            //Data.GetWords();
            Reset();
        }

        void Reset() {
			canvas.Children.Clear();
			row = 0;
			SetN();
			game = new Game(allWords[N]);

			NextWord();
		}

		private void SetN() {
			if (!int.TryParse(NumberBox.Text, out var n)) {
				NumberBox.Text = "5";
				n = 5;
			}
			N = n;
			Width = 75 * N + 220;
		}

		void NextWord() {
            currentWord = game.NextWord();
            if (string.IsNullOrEmpty(currentWord)) {
                MessageBox.Show("No matches");
                Reset();
                return;
            }
            PrintBestWords(game.GetBestWords());
            PrintUsefulWords(game.GetUsefulWords());
            DrawRow();
        }

        void PrintBestWords(List<Word> words) {
            WordsStackPanel.Children.Clear();
            foreach (var w in words) {
                PrintWord(w);
			}
		}

		void PrintUsefulWords(List<Word> words) {
            WordsStackPanel.Children.Add(new Label());
            foreach (var w in words) {
                PrintWord(w);
            }
        }

		private void PrintWord(Word w) {
			Label l = new Label();
			l.Margin = new Thickness(-4);
			l.FontSize = 15;
			l.FontWeight = FontWeights.Bold;
			l.Content = w.ToStringLong();
			l.MouseDown += (o, e) => {
				currentWord = w.word;
				row--;
				DrawRow();
			};
			WordsStackPanel.Children.Add(l);
		}

        List<Rectangle> rects = new List<Rectangle>();

        public void DrawRow() {
            rects.Clear();

            char[] colors = game.GetColors(currentWord);

            for (int i = 0; i < N; i++) {
                Rectangle rect = new Rectangle();

                rect.Fill = colors[i] == 'g' ? Green : colors[i] == 'y' ? Yellow : Grey;
                rect.StrokeThickness = 2;
                rect.Stroke = Brushes.Black;
                rect.Width = 60;
                rect.Height = 60;
                rect.MouseLeftButtonDown += rectangle_MouseLeftButtonDown;

                canvas.Children.Add(rect);
                Canvas.SetTop(rect, 20 + 75*row);
                Canvas.SetLeft(rect, 10 + 75 * i);
                rects.Add(rect);

                Label l = new Label();
                l.Content = currentWord[i];
                l.FontSize = 40;
                l.Foreground = new SolidColorBrush(Colors.White);
                l.FontWeight = FontWeights.Bold;
                l.IsHitTestVisible = false;
                l.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size s = l.DesiredSize;

                canvas.Children.Add(l);
                Canvas.SetTop(l, 15 + 75 * row);
                Canvas.SetLeft(l, 40 + 75 * i - s.Width/2);
            }
			row++;
        }
        void rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var rect = (Rectangle)sender;
            if(rect.Fill == Grey) {
                rect.Fill = Yellow;
            }
            else if (rect.Fill == Yellow) {
                rect.Fill = Green;
            }
            else if (rect.Fill == Green) {
                rect.Fill = Grey;
            }
        }

        private void SetClick(object sender, RoutedEventArgs e) {
            var colors = new char[N];
            for (int i = 0; i < N; i++) {
                if (rects[i].Fill == Green) colors[i] = 'g';
                if (rects[i].Fill == Yellow) colors[i] = 'y';
                if (rects[i].Fill == Grey) colors[i] = 'r';
            }
            game.SetWord(currentWord, colors);
            NextWord();
        }


        private void RestartClick(object sender, RoutedEventArgs e) {
            Reset();
        }
	}
}
