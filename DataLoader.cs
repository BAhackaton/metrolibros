using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using MetroLibros.Model;

namespace MetroLibros
{
    public class DataLoader
    {
        private const int MAX = 100000;

        internal static List<Model.Libro> CargarLibros()
        {
            var resource = Application.GetResourceStream(new Uri("LibrosParsed.txt", UriKind.Relative));
            var sReader = new StreamReader(resource.Stream);
            var text = sReader.ReadToEnd();

            var res = new List<Libro>();

            foreach (var line in text.Split('|'))
            {
                if (res.Count <= MAX)
                {
                    var p = line.Split(';');
                    if (p.Length == 5)
                    {
                        var aut = p[4];

                        var i = aut.IndexOf("^d");

                        if (i > -1)
                            aut = aut.Substring(0, i);

                        var libro = new Libro() { ISBN = "ISBN: " + p[0], Id = p[1], Titulo = p[2], Categoria = p[3], Autor = aut };
                        if (!string.IsNullOrEmpty(libro.Titulo))
                            res.Add(libro);
                    }
                }
            }

            return res;
        }
    }
}
