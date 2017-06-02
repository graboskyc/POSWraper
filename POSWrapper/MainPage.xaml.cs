using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace POSWrapper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;

        public MainPage()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            printDoc.AddPages += AddPages;
        }

        #region printing
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            // As I only want to print one Rectangle, so I set the count to 1
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Provide a UIElement as the print preview.
            printDoc.SetPreviewPage(e.PageNumber, this.RectangleToPrint);
        }


        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(this.RectangleToPrint);

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }


        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
            
        }
        #endregion printing


        #region webviewcontrol
        private async void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await webView.InvokeScriptAsync("eval", new string[] { "window.alert = function (AlertMessage) {window.external.notify(AlertMessage)}" });
            await webView.InvokeScriptAsync("eval", new string[] { "$('#btn_Print').hide();" });
            await webView.InvokeScriptAsync("eval", new string[] { "$('#btn_Clear').hide();" });
            await webView.InvokeScriptAsync("eval", new string[] { "$('#btn_Save').hide();" });
        }

        private async void webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(e.Value);
            await dialog.ShowAsync();
        }
        #endregion webviewcontrol


        #region buttons
        private async void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> scriptArgs = new string[] { "$('#printsheet').remove();" };
            string x = await webView.InvokeScriptAsync("eval", scriptArgs);
            webView.Width = maingrid.Width;
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            webView.HorizontalAlignment = HorizontalAlignment.Stretch;
            webView.VerticalAlignment = VerticalAlignment.Stretch;
            webView.Width = maingrid.Width;
            webView.Refresh();
            
        }

        private async void btn_print_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> scriptArgs = new string[] { "window.printRecpt();" };
            string x = await webView.InvokeScriptAsync("eval", scriptArgs);

            int width;
            int height;
            // get the total width and height
            var widthString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            if (!int.TryParse(widthString, out width))
            {
                throw new Exception("Unable to get page width");
            }
            if (!int.TryParse(heightString, out height))
            {
                throw new Exception("Unable to get page height");
            }

            // resize the webview to the content
            webView.Height = height;
            webView.Width = 800;
            RectangleToPrint.Height = height;
            RectangleToPrint.Width = 800;

            WebViewBrush wvBrush = new WebViewBrush();
            wvBrush.SetSource(webView);
            wvBrush.Redraw();
            RectangleToPrint.Fill = wvBrush;

            await Task.Delay(500);

            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
        }

        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> scriptArgs = new string[] { "window.saveRecpt();" };
            string x = await webView.InvokeScriptAsync("eval", scriptArgs);
        }

        #endregion buttons


    }
}
