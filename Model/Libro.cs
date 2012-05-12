namespace MetroLibros.Model
{
    public class Libro
    {

        public string Id { get; set; }

        public string Titulo { get; set; }

        public string ISBN { get; set; }

        public string Autor { get; set; }
        public string AutorFirst { get { return Autor.Substring(0, 1); } }

        public string TituloFirst { get { return Titulo.Substring(0, 1); } }

        public string Categoria { get; set; }
        public string CategoriaFirst { get { return Categoria.Substring(0, 1); } }
    }
}
