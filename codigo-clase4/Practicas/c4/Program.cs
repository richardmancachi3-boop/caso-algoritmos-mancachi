using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;

class Program
{
    // =====================================
    // 1. BUSQUEDA LINEAL
    // =====================================
    static int BusquedaLineal(int[] datos, int buscado)
    {
        for (int i = 0; i < datos.Length; i++)
        {
            if (datos[i] == buscado)
            {
                return i;
            }
        }

        return -1;
    }

    // =====================================
    // 2. BUSQUEDA BINARIA
    // =====================================
    static int BusquedaBinaria(int[] datos, int buscado)
    {
        int inicio = 0;
        int fin = datos.Length - 1;

        while (inicio <= fin)
        {
            int medio = inicio + (fin - inicio) / 2;

            if (datos[medio] == buscado)
            {
                return medio;
            }

            if (datos[medio] < buscado)
            {
                inicio = medio + 1;
            }
            else
            {
                fin = medio - 1;
            }
        }

        return -1;
    }

    // =====================================
    // 3. MEDIR BUSQUEDA LINEAL
    // =====================================
    static double MedirLineal(int[] datos, int[] claves)
    {
        var reloj = Stopwatch.StartNew();

        long acumulado = 0;

        foreach (int k in claves)
        {
            acumulado += BusquedaLineal(datos, k);
        }

        reloj.Stop();

        GC.KeepAlive(acumulado);

        return reloj.Elapsed.TotalMilliseconds;
    }

    // =====================================
    // 4. MEDIR BUSQUEDA BINARIA
    // =====================================
    static double MedirBinaria(int[] datos, int[] claves)
    {
        var reloj = Stopwatch.StartNew();

        long acumulado = 0;

        foreach (int k in claves)
        {
            acumulado += BusquedaBinaria(datos, k);
        }

        reloj.Stop();

        GC.KeepAlive(acumulado);

        return reloj.Elapsed.TotalMilliseconds;
    }

    // =====================================
    // 5. PROGRAMA PRINCIPAL
    // =====================================
    static void Main()
    {
        const int N = 1000000;
        const int CONSULTAS = 1000;
        const int CORRIDAS = 3;

        Console.WriteLine("COMPARACION: BUSQUEDA LINEAL CONTRA BINARIA");
        Console.WriteLine("============================================");

        // CREAR UN MILLON DE NUMEROS ORDENADOS
        int[] datos = new int[N];

        for (int i = 0; i < N; i++)
        {
            datos[i] = i * 2;
        }

        // GENERAR 1000 CLAVES QUE EXISTEN
        Random azar = new Random(21);

        int[] claves = new int[CONSULTAS];

        for (int i = 0; i < CONSULTAS; i++)
        {
            claves[i] = azar.Next(0, N) * 2;
        }

        // COMPROBAR QUE LAS DOS BUSQUEDAS
        // ENCUENTRAN LA MISMA POSICION
        int pruebaL = BusquedaLineal(datos, claves[0]);
        int pruebaB = BusquedaBinaria(datos, claves[0]);

        Console.WriteLine("\nDATOS PREPARADOS");
        Console.WriteLine("Cantidad de elementos: " + N);
        Console.WriteLine("Cantidad de consultas: " + CONSULTAS);
        Console.WriteLine("Primera consulta: " + claves[0]);
        Console.WriteLine("Posicion lineal: " + pruebaL);
        Console.WriteLine("Posicion binaria: " + pruebaB);

        // CALENTAMIENTO
        MedirLineal(datos, claves);
        MedirBinaria(datos, claves);

        Console.WriteLine("\nCALENTAMIENTO COMPLETADO");
        Console.WriteLine("Iniciando tres corridas medidas...");

        // REALIZAR TRES CORRIDAS
        double[] tiemposLineal = new double[CORRIDAS];
        double[] tiemposBinaria = new double[CORRIDAS];

        for (int i = 0; i < CORRIDAS; i++)
        {
            tiemposLineal[i] = MedirLineal(datos, claves);
            tiemposBinaria[i] = MedirBinaria(datos, claves);
        }

        // CALCULAR PROMEDIOS
        double sumaL = 0;
        double sumaB = 0;

        for (int i = 0; i < CORRIDAS; i++)
        {
            sumaL += tiemposLineal[i];
            sumaB += tiemposBinaria[i];
        }

        double promedioL = sumaL / CORRIDAS;
        double promedioB = sumaB / CORRIDAS;

        double cociente = promedioL / promedioB;

        // MOSTRAR RESULTADOS
        Console.WriteLine("\nRESULTADOS DE LAS CORRIDAS");
        Console.WriteLine("--------------------------------------------");

        Console.WriteLine("Corrida     Lineal (ms)     Binaria (ms)");

        for (int i = 0; i < CORRIDAS; i++)
        {
            Console.WriteLine(
                $"{i + 1,-12}{tiemposLineal[i],-16:F3}{tiemposBinaria[i],-16:F3}");
        }

        Console.WriteLine("--------------------------------------------");

        Console.WriteLine("\nPROMEDIOS");
        Console.WriteLine("Busqueda lineal: " + promedioL.ToString("F3") + " ms");
        Console.WriteLine("Busqueda binaria: " + promedioB.ToString("F3") + " ms");

        Console.WriteLine("\nCOCIENTE");
        Console.WriteLine("La busqueda binaria fue aproximadamente "
            + cociente.ToString("F1") + " veces mas rapida.");

        // GUARDAR RESULTADOS EN MARKDOWN
        string ruta = "resultados.md";

        string contenido =
            "# Comparacion: busqueda lineal contra binaria\n\n" +
            "- Lenguaje: C#\n" +
            "- Elementos: 1.000.000\n" +
            "- Consultas: 1.000\n" +
            "- Protocolo: 1 calentamiento + 3 corridas + promedio\n\n" +
            "| Corrida | Lineal (ms) | Binaria (ms) |\n" +
            "|---|---:|---:|\n";

        for (int i = 0; i < CORRIDAS; i++)
        {
            contenido +=
                $"| {i + 1} | {tiemposLineal[i].ToString("F3", CultureInfo.InvariantCulture)}" +
                $" | {tiemposBinaria[i].ToString("F3", CultureInfo.InvariantCulture)} |\n";
        }

        contenido +=
            $"\n| **Promedio** | **{promedioL.ToString("F3", CultureInfo.InvariantCulture)}**" +
            $" | **{promedioB.ToString("F3", CultureInfo.InvariantCulture)}** |\n\n" +
            $"**Cociente:** {cociente.ToString("F1", CultureInfo.InvariantCulture)} veces.\n\n" +
            "## Conclusion sobre mi proceso H1\n\n" +
            "Conviene ordenar una sola vez los registros cuando se realizaran muchas " +
            "consultas sobre los mismos datos, porque la busqueda binaria permite " +
            "encontrar informacion mas rapidamente.\n\n" +
            "Si se realizan pocas consultas o los datos cambian constantemente, " +
            "puede convenir la busqueda lineal para evitar el costo de ordenar primero.\n";

        File.WriteAllText(ruta, contenido);

        Console.WriteLine("\nARCHIVO GUARDADO");
        Console.WriteLine("Ubicacion: " + Path.GetFullPath(ruta));

        Console.WriteLine("\n============================================");
        Console.WriteLine("FIN DEL PROGRAMA");
    }
}
