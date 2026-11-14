// CLASE 5 · EL TORNEO DE ORDENAMIENTOS — divide y conquista contra los cuadráticos
// Trazas y conteos son exactos (iguales en toda máquina). Los tiempos se miden con el protocolo
// (calentar, repetir, promediar) y varían con la máquina.

using System.Diagnostics;

namespace C5.Torneo;

public static class Demo
{
    // ---------- MERGESORT: dividir en mitades, ordenar cada mitad, MEZCLAR ----------
    private static long MergeSort(int[] a)
    {
        long c = 0;
        Mergear(a, new int[a.Length], 0, a.Length - 1, ref c);
        return c;
    }

    private static void Mergear(int[] a, int[] tmp, int ini, int fin, ref long c)
    {
        if (ini >= fin) return;
        int medio = ini + (fin - ini) / 2;
        Mergear(a, tmp, ini, medio, ref c);
        Mergear(a, tmp, medio + 1, fin, ref c);
        int i = ini, j = medio + 1, k = ini;
        while (i <= medio && j <= fin)
        {
            c++;
            if (a[i] <= a[j]) tmp[k++] = a[i++]; else tmp[k++] = a[j++];
        }
        while (i <= medio) tmp[k++] = a[i++];
        while (j <= fin) tmp[k++] = a[j++];
        for (k = ini; k <= fin; k++) a[k] = tmp[k];
    }

    // ---------- QUICKSORT: elegir un pivote, dejar menores a un lado y mayores al otro ----------
    private static long QuickSort(int[] a, bool pivoteAlMedio = false)
    {
        long c = 0;
        Rapido(a, 0, a.Length - 1, pivoteAlMedio, ref c);
        return c;
    }

    private static void Rapido(int[] a, int ini, int fin, bool alMedio, ref long c)
    {
        if (ini >= fin) return;
        int p = alMedio ? ini + (fin - ini) / 2 : fin;     // dónde está el pivote
        (a[p], a[fin]) = (a[fin], a[p]);                   // el pivote se guarda al final
        int pivote = a[fin];
        int i = ini;                                       // frontera: lo de la izquierda es menor que el pivote
        for (int j = ini; j < fin; j++)
        {
            c++;
            if (a[j] < pivote) { (a[i], a[j]) = (a[j], a[i]); i++; }
        }
        (a[i], a[fin]) = (a[fin], a[i]);                   // el pivote queda en su lugar definitivo
        Rapido(a, ini, i - 1, alMedio, ref c);
        Rapido(a, i + 1, fin, alMedio, ref c);
    }

    // ---------- INSERCIÓN (el cuadrático de la clase 4, para comparar) ----------
    private static long Insercion(int[] a)
    {
        long c = 0;
        for (int i = 1; i < a.Length; i++)
        {
            int actual = a[i], j = i - 1;
            while (j >= 0)
            {
                c++;
                if (a[j] > actual) { a[j + 1] = a[j]; j--; } else break;
            }
            a[j + 1] = actual;
        }
        return c;
    }

    // ---------- ORDENAR SIN COMPARAR: contar cuántas veces aparece cada valor ----------
    private static void OrdenarContando(int[] a, int maximo)
    {
        var cuenta = new int[maximo + 1];
        foreach (var x in a) cuenta[x]++;
        int k = 0;
        for (int v = 0; v <= maximo; v++)
            for (int veces = cuenta[v]; veces > 0; veces--) a[k++] = v;
    }

    private static int[] AlAzar(int n, int semilla, int tope = 1_000_000)
    {
        var azar = new Random(semilla);
        var d = new int[n];
        for (int i = 0; i < n; i++) d[i] = azar.Next(0, tope);
        return d;
    }

    private static void Verificar(int[] a, string quien)
    {
        for (int i = 1; i < a.Length; i++)
            if (a[i - 1] > a[i]) throw new Exception($"{quien} NO dejó el arreglo ordenado (posición {i})");
    }

    // ======================= LAS PARTES DE LA DEMO =======================

    public static void Correr()
    {
        Trazas(); Contar(); Torneo(); PeorCaso(); SinComparar();
    }

    public static void Trazas()
    {
        Console.WriteLine("PARTE A · A MANO, PASO A PASO (8 números)");
        var datos = new[] { 5, 2, 9, 1, 7, 3, 8, 4 };
        Console.WriteLine($"MergeSort sobre [{string.Join(", ", datos)}]:");
        TrazaMerge((int[])datos.Clone(), 0, datos.Length - 1, 1);
        Console.WriteLine();
        Console.WriteLine($"QuickSort (pivote = último) sobre [{string.Join(", ", datos)}]:");
        var q = (int[])datos.Clone();
        TrazaRapido(q, 0, q.Length - 1);
        Console.WriteLine($"  resultado: [{string.Join(", ", q)}]");
        Console.WriteLine();
    }

    private static void TrazaMerge(int[] a, int ini, int fin, int nivel)
    {
        string sangria = new string(' ', nivel * 2);
        var tramo = a[ini..(fin + 1)];
        if (ini >= fin) return;
        Console.WriteLine($"{sangria}dividir [{string.Join(", ", tramo)}]");
        int medio = ini + (fin - ini) / 2;
        TrazaMerge(a, ini, medio, nivel + 1);
        TrazaMerge(a, medio + 1, fin, nivel + 1);
        long c = 0;
        Mergear(a, new int[a.Length], ini, fin, ref c);
        Console.WriteLine($"{sangria}mezclar  → [{string.Join(", ", a[ini..(fin + 1)])}]");
    }

    private static void TrazaRapido(int[] a, int ini, int fin)
    {
        if (ini >= fin) return;
        int pivote = a[fin];
        int i = ini;
        for (int j = ini; j < fin; j++)
            if (a[j] < pivote) { (a[i], a[j]) = (a[j], a[i]); i++; }
        (a[i], a[fin]) = (a[fin], a[i]);
        Console.WriteLine($"  pivote {pivote}: [{string.Join(", ", a)}]  (el {pivote} ya está en su lugar: posición {i})");
        TrazaRapido(a, ini, i - 1);
        TrazaRapido(a, i + 1, fin);
    }

    public static void Contar()
    {
        Console.WriteLine("PARTE B · CONTAR COMPARACIONES (datos al azar; n x2)");
        Console.WriteLine();
        Console.WriteLine($"{"n",8} {"inserción",15} {"x",6} {"MergeSort",12} {"x",6} {"QuickSort",12} {"x",6}");
        long ai = 0, am = 0, aq = 0;
        foreach (var n in new[] { 1_000, 2_000, 4_000, 8_000, 16_000 })
        {
            var ci = Insercion(AlAzar(n, 5));
            var cm = MergeSort(AlAzar(n, 5));
            var cq = QuickSort(AlAzar(n, 5));
            string R(long x, long ant) => ant > 0 ? $"x{(double)x / ant:0.0}" : "—";
            Console.WriteLine($"{n,8:N0} {ci,15:N0} {R(ci, ai),6} {cm,12:N0} {R(cm, am),6} {cq,12:N0} {R(cq, aq),6}");
            ai = ci; am = cm; aq = cq;
        }
        Console.WriteLine();
        Console.WriteLine("Doble de datos: la inserción x4 (n²); MergeSort y QuickSort un poco más de x2 (n log n).");
        Console.WriteLine();
    }

private static double[] MedirTres(
    int[] original,
    Func<int[], long> ordenar)
{
    double UnaCorrida()
    {
        var copia = (int[])original.Clone();

        var reloj = Stopwatch.StartNew();

        ordenar(copia);

        reloj.Stop();

        Verificar(copia, "Torneo");

        return reloj.Elapsed.TotalMilliseconds;
    }

    // Una corrida de calentamiento
    UnaCorrida();

    // Tres corridas medidas
    double[] tiempos = new double[3];

    for (int i = 0; i < 3; i++)
    {
        tiempos[i] = UnaCorrida();
    }

    return tiempos;
}
   public static void Torneo()
{
    Console.WriteLine("==============================================");
    Console.WriteLine("TORNEO DE ALGORITMOS DE ORDENAMIENTO - C#");
    Console.WriteLine("==============================================");

    Console.WriteLine("3 corridas por algoritmo y tamano.");
    Console.WriteLine("1 calentamiento + 3 corridas medidas + promedio.");

    foreach (var n in new[] { 10_000, 20_000, 40_000, 80_000 })
    {
        int[] datos = AlAzar(n, 9);

        double[] insercion = MedirTres(datos, a => Insercion(a));
        double[] merge = MedirTres(datos, a => MergeSort(a));
        double[] quick = MedirTres(datos, a => QuickSort(a));
        double[] integrado = MedirTres(datos, a =>
        {
            Array.Sort(a);
            return 0L;
        });

        Console.WriteLine("\n==============================================");
        Console.WriteLine($"TAMANO: {n:N0} ELEMENTOS");
        Console.WriteLine("==============================================");

        Console.WriteLine(
            "{0,-18} {1,12} {2,12} {3,12} {4,12}",
            "Algoritmo", "Corrida 1", "Corrida 2",
            "Corrida 3", "Promedio");

        Console.WriteLine(new string('-', 70));

        Mostrar("Insertion Sort", insercion);
        Mostrar("Merge Sort", merge);
        Mostrar("Quick Sort", quick);
        Mostrar("Array.Sort()", integrado);
    }

    Console.WriteLine("\nFIN DEL TORNEO");
}

private static void Mostrar(string nombre, double[] tiempos)
{
    double promedio = (tiempos[0] + tiempos[1] + tiempos[2]) / 3;

    Console.WriteLine(
        "{0,-18} {1,12:F3} {2,12:F3} {3,12:F3} {4,12:F3}",
        nombre,
        tiempos[0],
        tiempos[1],
        tiempos[2],
        promedio);
}

    private static double Promedio(int[] original, Func<int[], long> orden)
    {
        double Una()
        {
            var copia = (int[])original.Clone();
            var r = Stopwatch.StartNew();
            orden(copia);
            r.Stop();
            Verificar(copia, "algoritmo del torneo");
            return r.Elapsed.TotalMilliseconds;
        }
        Una();
        double suma = 0;
        for (int c = 0; c < 3; c++) suma += Una();
        return suma / 3;
    }

    public static void PeorCaso()
    {
        Console.WriteLine("PARTE D · EL PEOR CASO DE QUICKSORT (n = 5.000, comparaciones)");
        Console.WriteLine();
        int n = 5_000;
        var ordenado = Enumerable.Range(0, n).ToArray();
        var azar = AlAzar(n, 5);
        Console.WriteLine($"{"datos",12} {"pivote = último",18} {"pivote = medio",16}");
        foreach (var (nombre, d) in new[] { ("al azar", azar), ("ya ordenado", ordenado) })
        {
            var ultimo = QuickSort((int[])d.Clone(), pivoteAlMedio: false);
            var medio = QuickSort((int[])d.Clone(), pivoteAlMedio: true);
            Console.WriteLine($"{nombre,12} {ultimo,18:N0} {medio,16:N0}");
        }
        Console.WriteLine();
        Console.WriteLine("Con el pivote en el último y los datos ya ordenados, cada partición apenas descarta un elemento: n²/2.");
        Console.WriteLine("Elegir bien el pivote (al medio, al azar, mediana de tres) devuelve el n log n.");
        Console.WriteLine();
    }

    public static void SinComparar()
    {
        Console.WriteLine("PARTE E · ORDENAR SIN COMPARAR (noción): contar cuántas veces aparece cada valor");
        Console.WriteLine("5.000.000 de edades entre 0 y 99. PROTOCOLO: 1 corrida de calentamiento + 3 corridas medidas + promedio.");
        Console.WriteLine();
        var edades = AlAzar(5_000_000, 3, tope: 100);
        double pc = Promedio(edades, a => { OrdenarContando(a, 99); return 0; });
        double ps = Promedio(edades, a => { Array.Sort(a); return 0; });
        var control = (int[])edades.Clone(); OrdenarContando(control, 99);
        var esperado = (int[])edades.Clone(); Array.Sort(esperado);
        Console.WriteLine($"  Contando valores: {pc,8:N1} ms");
        Console.WriteLine($"  Array.Sort      : {ps,8:N1} ms");
        Console.WriteLine($"  ¿Mismo resultado? {(control.SequenceEqual(esperado) ? "sí" : "NO")}");
        Console.WriteLine();
        Console.WriteLine("Sin comparar, el costo es ~n + k (k = cuántos valores distintos posibles). Solo sirve cuando k es chico.");
    }
}
