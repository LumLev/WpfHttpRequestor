using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HttpClient Req;

        private bool RequestWithBody { get; set; } = false;
        public MainWindow()
        {
            Req = new HttpClient();
            InitializeComponent();
            HideAll();
            FillingHttpMethods();
            FillingEncoders();        
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            HttpRequestMessage HRM = new HttpRequestMessage();
            // If with body text request
            if (RequestWithBody)
            {
                HRM.Content = new StringContent(TheRequestBodyTextBox.Text, (Encoding)TheComboEncodingBox.SelectedValue, TheTextBoxContentType.Text);
            }
            try
            {
                TheSubmit.IsEnabled = false; TheSubmit.Background = new SolidColorBrush(Colors.Black);
                TheSubmit.Content = "Loading";
                HRM.RequestUri = new UriBuilder(TheTextBox.Text).Uri;
                int i = TheComboBox.SelectedIndex;
                HRM.Method = (HttpMethod)TheComboBox.SelectedItem;
                HttpResponseMessage Hresponse = await Req.SendAsync(HRM);
                if (Hresponse.IsSuccessStatusCode)
                {
                    string x = await Hresponse.Content.ReadAsStringAsync();
                    TheTextBlock.Text = x;
                }
                else { TheComboBox.Text = $"Request failed with: {Hresponse.StatusCode}. \nReason Phrase: {Hresponse.ReasonPhrase}"; }
                TheComboBox.SelectedIndex = i;
            } catch (Exception ex) { TheTextBlock.Text = ex.Message; Debug.WriteLine(ex.Message); }
            TheSubmit.Content = "Submitted";
            TheSubmit.IsEnabled = true; TheSubmit.Background = new SolidColorBrush(Colors.DarkGreen);
        }



        private void ShowAll()
        {
            TheComboEncodingBox.Visibility = Visibility.Visible;
            TheLabelContent.Visibility = Visibility.Visible;
            TheTextBoxContentType.Visibility = Visibility.Visible;
            TheRequestBodyTextBox.Visibility = Visibility.Visible;
        }

        private void HideAll()
        {
            TheComboEncodingBox.Visibility = Visibility.Hidden;
            TheLabelContent.Visibility = Visibility.Hidden;
            TheTextBoxContentType.Visibility = Visibility.Hidden;
            TheRequestBodyTextBox.Visibility = Visibility.Collapsed;
        }

        private void TheButtonAddBody_Click(object sender, RoutedEventArgs e)
        {
            
            if (RequestWithBody is false)
            {
               
                ShowAll();
                RequestWithBody = true;
                TheButtonAddBody.Content = "Clear Body";
                TheButtonAddBody.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else { RequestWithBody = false; HideAll(); TheButtonAddBody.Content = "Request Body"; TheButtonAddBody.Background = new SolidColorBrush(Colors.RoyalBlue); }
        }



        private void FillingHttpMethods()
        {
            PropertyInfo[] httpMethods = typeof(HttpMethod).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo property in httpMethods)
            {
                var httpMethod = (HttpMethod?)property.GetValue(null);
                if (httpMethod is not null) { TheComboBox.Items.Add(httpMethod); }
                TheComboBox.SelectedIndex = 0;
            }
        }

        private void FillingEncoders()
        {
            int c = 0;
            PropertyInfo[] EncodingTypes = typeof(Encoding).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo Enc in EncodingTypes)
            {
                TheComboEncodingBox.Items.Add(Enc);
                    if (Enc.Name.Contains("UTF8")) { TheComboEncodingBox.SelectedIndex = c; }
                c++;
            }
        }
    }
}
