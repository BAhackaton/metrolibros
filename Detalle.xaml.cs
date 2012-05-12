using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using MetroLibros.Model;
using System;
using Microsoft.Phone.Tasks;

namespace MetroLibros
{
    public partial class Detalle : PhoneApplicationPage
    {
        private Libro actual;

        private const string URL = "http://catalogo.bibliotecas.gob.ar/pergamo/opac/cgi-bin/pgopac.cgi?VDOC=1.{0}-null#Ejemplares";

        public Detalle()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var isbn = NavigationContext.QueryString["isbn"];
            actual = App.Data.First(item => item.ISBN == isbn);
            mainPivot.Title = actual.Titulo.ToUpper();
            lblIsbn.Text = actual.ISBN.Replace("ISBN: ", "");
            lblAutor.Text = actual.Autor;
            lblCategoria.Text = actual.Categoria;

            webBrowser1.Navigate(new Uri(string.Format(URL, actual.Id)));

            base.OnNavigatedTo(e);
        }

        private void mainPivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {           
            if (mainPivot.SelectedIndex == 4)
            {
                imgCover.Source = new BitmapImage(new Uri(string.Format("http://covers.openlibrary.org/b/isbn/{0}-S.jpg", actual.ISBN.Replace("ISBN: ", "").Replace("-", ""))));
            }
        }

        private void textBlock2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var email = new EmailComposeTask
                            {
                                Subject = actual.Titulo,
                                Body =
                                    "Estoy Leyendo: " + actual.Titulo + ", lo encontré gratis en Metro Libros BsAs." +
                                    "Buenos Aires Ciudad. En todo estás Vos"
                            };
            email.Show();
        }
    }
}