using System.Data.SqlTypes;
using System.Diagnostics;
using System.Security.Policy;

namespace DemoIntroAsync2788307
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencia");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucuion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            var sw = new Stopwatch();
            sw.Start();

            foreach(var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Secuencial - duracion en segundos: {0}", sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();
            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>{

                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("Paralelo - duracion en segundos: {0}", sw.ElapsedMilliseconds / 1000.0);
            sw.Stop();

            //var tareas = new List<Task>()
            //{
            //    RealizarProcesamientoLargoA(),
            //       RealizarProcesamientoLargoB(),
            //      RealizarProcesamientoLargoC()

            //};
            //await Task.WhenAll(tareas);

            sw.Stop();

            var duracion = $"El programa se ejecuto en {sw.ElapsedMilliseconds / 1000.0} segundos";
            Console.WriteLine(duracion);

            pictureBox1.Visible = false;

        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using(var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach ( var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucuion(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);

            }
            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }


        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(3000);
            return "Felipe";
        }
        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");
        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }


        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for(int i = 0; i < 7;  i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Primer Presidente {i}.jpg",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8b/Manuel_Jos%C3%A9_Arce_1.jpg/800px-Manuel_Jos%C3%A9_Arce_1.jpg"
                    });
                imagenes.Add(
                   new Imagen()
                   {
                      Nombre = $"Añil {i}.jpg",
                       URL = "https://upload.wikimedia.org/wikipedia/commons/8/83/Indigofera_tinctoria0.jpg"
                   });
                imagenes.Add(
                   new Imagen()
                   {
                     Nombre = $"Tazumal {i}.jpg",

                             URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/df/Templo_tazumal.jpg/800px-Templo_tazumal.jpg"
                     });

            }

            return imagenes;
        }
      
    }
}
