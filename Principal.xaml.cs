using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MetroLibros.Model;
using System.Linq;
using System.Windows.Controls;
using WP7_Barcode_Library;

namespace MetroLibros
{
    public partial class Principal : PhoneApplicationPage
    {

        public Principal()
        {
            InitializeComponent();

            CargarLibros();
        }

        private void CargarLibros()
        {
            App.Data = DataLoader.CargarLibros();

            //this.lstLibros.ItemsSource = data.OrderBy(libro => libro.Titulo);

            var librosPorTit = from libro in App.Data
                               group libro by libro.TituloFirst into c
                               orderby c.Key
                               select new Group<Libro>(c.Key, c);

            this.lstLibrosTitulo.ItemsSource = librosPorTit;

            var librosPorAutor = from libro in App.Data
                                 group libro by libro.AutorFirst into c
                                 orderby c.Key
                                 select new Group<Libro>(c.Key, c);

            this.lstLibrosAut.ItemsSource = librosPorAutor;

        }

        private void TextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var txt = sender as TextBlock;
            if (txt != null)
                NavigationService.Navigate(new Uri("/Detalle.xaml?isbn=" + txt.Tag.ToString(), UriKind.Relative));
        }

        private void btnISBN_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //WP7BarcodeManager.ScanMode = com.google.zxing.BarcodeFormat.UPC_EAN;

                //if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator) //Use Barcode Sample Chooser "Task" if bypass camera setting is active (Emulator/demo mode)
                //{
                //    NavigationService.Navigating += new NavigatingCancelEventHandler(NavigationService_Navigating);
                //    NavigationService.Navigate(BarcodeSampleItemManager.BarcodeChooserURI); //Navigate to barcode chooser page located in WP7_Barcode_Library.WP7Code
                //}
                //else //Use BarcodeManager to start camera and scan image
                //{
                //    WP7BarcodeManager.ScanBarcode(BarcodeResults_Finished); //Provide callback method
                //}
                WP7BarcodeManager.ScanMode = com.google.zxing.BarcodeFormat.EAN_13;
                WP7BarcodeManager.ScanBarcode((result) =>
                {
                    var book = App.Data.FirstOrDefault(item => item.ISBN == "ISBN: 950-557-612-9");
                    NavigationService.Navigate(new Uri("/Detalle.xaml?isbn=" + book.ISBN, UriKind.Relative));

                    //if (result.State == CaptureState.Success)
                    //{
                    //    var isbn = "ISBN: " + result.BarcodeText;
                    //    var book = App.Data.FirstOrDefault(item => item.ISBN.Replace("-", "") == isbn);
                    //    if (book != null)
                    //    {
                    //        NavigationService.Navigate(new Uri("/Detalle.xaml?isbn=" + book.ISBN, UriKind.Relative));
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("El Libro no se encuentra en la Base de Datos");
                    //    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("El ISBN no fué leído correctamente");
                    //}
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing image.", ex);
            }
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && NavigationService.CurrentSource == BarcodeSampleItemManager.BarcodeChooserURI)
            {
                NavigationContext.QueryString["LOADSAMPLE"] = "TRUE";//Set flag to load sample
            }
            else
            {
                NavigationContext.QueryString.Remove("LOADSAMPLE");
            }
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Detect if page is navigating back from BarcodeSampleChooser and start processing
            try
            {
                string strFlag;
                if (NavigationContext.QueryString.TryGetValue("LOADSAMPLE", out strFlag))
                {
                    if (BarcodeSampleItemManager.SelectedItem != null)
                    {
                        WP7BarcodeManager.ScanBarcode(BarcodeSampleItemManager.SelectedItem.FileURI, new Action<BarcodeCaptureResult>(this.BarcodeResults_Finished));
                    }
                    else
                    {
                        MessageBox.Show("Error: Sample chooser canceled");
                    }
                    NavigationContext.QueryString.Remove("LOADSAMPLE");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error processing sample image.", ex);
            }
        }

        public void BarcodeResults_Finished(BarcodeCaptureResult BCResults)
        {
            try
            {
                if (BCResults.State == CaptureState.Success && !string.IsNullOrEmpty(BCResults.BarcodeText))
                {
                    NavigationService.Navigate(new Uri("/Detalle.xaml?isbn=" + BCResults.BarcodeText, UriKind.Relative));
                }
                else
                {
                    Debug.WriteLine(BCResults.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Barcode Processing Error: {0}", ex.Message));
            }
        }

        private void btnTexto_Click(object sender, RoutedEventArgs e)
        {
            if (txtKeyword.Text != "")
            {
                Libro res = null;
                foreach (var l in App.Data)
                {
                    if (l.Titulo.ToLower().Contains(txtKeyword.Text.ToLower()))
                    {
                        res = l;
                    }
                }
                if (res != null)
                    NavigationService.Navigate(new Uri("/Detalle.xaml?isbn=" + res.ISBN, UriKind.Relative));
                else
                {
                    MessageBox.Show("El Libro no se encuentra en la Base de Datos");
                }
                txtKeyword.Text = "";
            }
        }

    }
}