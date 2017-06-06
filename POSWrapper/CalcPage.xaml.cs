using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Text;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace POSWrapper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalcPage : Page
    {
        private double _total = 0.00;
        private string _lastNumber = "0";
        StringBuilder _tapeSB = new StringBuilder();

        public CalcPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();

            view.TitleBar.BackgroundColor = Color.FromArgb(255, 174, 81, 255);
            view.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 174, 81, 255);
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonPressedForegroundColor = Color.FromArgb(255, 174, 81, 255);
            view.TitleBar.ButtonPressedBackgroundColor = Colors.White;
            view.TitleBar.ButtonHoverBackgroundColor = Colors.White;
            view.TitleBar.ButtonHoverForegroundColor = Color.FromArgb(255, 174, 81, 255);
        }


        #region buttons


        private void btn_hamburger_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        #endregion buttons

        private void btn_key_Click(object sender, RoutedEventArgs e)
        {
            if(txt_lcd.Text == "0")
            {
                txt_lcd.Text = "";
            }
            else if(Convert.ToDouble(txt_lcd.Text) == _total)
            {
                txt_lcd.Text = "";
            }

            string s = (sender as Button).Content.ToString();
            txt_lcd.Text = txt_lcd.Text + s;

            _lastNumber = txt_lcd.Text;
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            if (txt_lcd.Text == "0")
            {
                _total = 0.00;
                txt_tape.Text = "";
                _tapeSB.Clear();
            }
            else
            {
                txt_lcd.Text = "0";
            }
        }

        private void btn_minus_Click(object sender, RoutedEventArgs e)
        {

            string newVal = txt_lcd.Text;

            if (Convert.ToDouble(txt_lcd.Text) == _total)
            {
                newVal = _lastNumber;
            }

            _tapeSB.Append("\n-" + newVal.PadLeft(10, ' '));
            txt_tape.Text = _tapeSB.ToString();
            _total = _total - (Convert.ToDouble(newVal));
            txt_lcd.Text = _total.ToString();

            txt_tape.Text = txt_tape.Text + "\n============================\n" + _total.ToString();
        }

        private void btn_plus_Click(object sender, RoutedEventArgs e)
        {
            string newVal = txt_lcd.Text;

            if (Convert.ToDouble(txt_lcd.Text) == _total)
            {
                newVal = _lastNumber;
            }

            _tapeSB.Append("\n+" + newVal.PadLeft(10, ' '));
            txt_tape.Text = _tapeSB.ToString();
            _total = _total + (Convert.ToDouble(newVal));
            txt_lcd.Text = _total.ToString();

            txt_tape.Text = txt_tape.Text + "\n============================\n" + _total.ToString();
        }

    }
}
