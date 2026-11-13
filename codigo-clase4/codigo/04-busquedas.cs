// CLASE 4 · BÚSQUEDA LINEAL CONTRA BINARIA — contar es exacto, medir es evidencia
// Parte A cuenta pasos (resultado idéntico en cualquier máquina).
// Parte B mide tiempos con el protocolo (calentar, repetir, promediar).
// Parte C responde: ¿cuándo vale la pena ORDENAR primero para poder buscar binario?

using System.Diagnostics;

namespace C4.Busquedas;

public static class Demo
{
    public static void Correr()
    {
        ParteA_Contar();
        ParteB_Medir();
        ParteC_PrecioDeOrdenar();
    }

    // LINEAL: recorre uno por uno. Devuelve la posición (o -1) y cuántos elementos miró.
    private static (int pos, long pasos) Lineal(int[] datos, int buscado)
    {
        long pasos = 0;
        for (int i = 0; i < datos.Length; i++)
        {
            pasos++;
            if (datos[i] == buscado) return (i, pasos);
        }
        return (-1, pasos);
    }

    // BINARIA: SOLO sirve si el arreglo está ORDENADO. Cada paso descarta la mitad.
    private static (int pos, long pasos) Binaria(int[] datos, int buscado)
    {
        long pasos = 0;
        int ini = 0, fin = datos.Length - 1;
        while (ini <= fin)
        {
            int medio = ini + (fin - ini) / 2;
            pasos++;
            if (datos[medio] == buscado) return (medio, pasos);
            if (datos[medio] < buscado) ini = medio + 1;
            else fin = medio - 1;
        }
        return (-1, pasos);
    }

    private static void ParteA_Contar()
    {
        Console.WriteLine("PARTE A · CONTAR PASOS (peor caso: el dato NO está)");
        Console.WriteLine();
        Console.WriteLine($"{"n",12} {"lineal",12} {"binaria",10} {"lineal / binaria",18}");
        foreach (var n in new[] { 1_000, 100_000, 1_000_000 })
        {
            var datos = new int[n];
            for (int i = 0; i < n; i++) datos[i] = i * 2;      // pares ordenados: 0, 2, 4, ...
            var l = Lineal(datos, 1);                          // 1 es impar: no está
            var b = Binaria(datos, 1);
            Console.WriteLine($"{n,12:N0} {l.pasos,12:N0} {b.pasos,10:N0} {"x" + (l.pasos / b.pasos).ToString("N0"),18}");
        }
        Console.WriteLine();
        Console.WriteLine("Con 1.000.000 de datos: la lineal mira un millón; la binaria, veinte.");
        Console.WriteLine();
    }

    private static void ParteB_Medir()
    {
        Console.WriteLine("PARTE B · MEDIR (1.000.000 de datos ordenados, 1.000 consultas)");
        Console.WriteLine("PROTOCOLO: 1 corrida de calentamiento + 3 corridas medidas + promedio.");
        Console.WriteLine();
        int n = 1_000_000, consultas = 1_000;
        var datos = new int[n];
        for (int i = 0; i < n; i++) datos[i] = i * 2;
        var azar = new Random(21);
        var claves = new int[consultas];
        for (int i = 0; i < consultas; i++) claves[i] = azar.Next(0, n) * 2;   // siempre existen

        double MedirLineal() { var r = Stopwatch.StartNew(); long acum = 0; foreach (var k in claves) acum += Lineal(datos, k).pos; r.Stop(); GC.KeepAlive(acum); return r.Elapsed.TotalMilliseconds; }
        double MedirBinaria() { var r = Stopwatch.StartNew(); long acum = 0; foreach (var k in claves) acum += Binaria(datos, k).pos; r.Stop(); GC.KeepAlive(acum); return r.Elapsed.TotalMilliseconds; }

        MedirLineal(); MedirBinaria();
        double sumaL = 0, sumaB = 0;
        for (int c = 0; c < 3; c++) { sumaL += MedirLineal(); sumaB += MedirBinaria(); }
        double pl = sumaL / 3, pb = sumaB / 3;

        Console.WriteLine($"  Lineal : {pl,9:N1} ms  para {consultas:N0} consultas");
        Console.WriteLine($"  Binaria: {pb,9:N3} ms  para {consultas:N0} consultas");
        Console.WriteLine();
        Console.WriteLine($"Misma respuesta, y la binaria fue ~x{pl / pb:N0} más rápida. Pero ojo: la binaria exige datos ORDENADOS.");
        Console.WriteLine();
    }

    private static void ParteC_PrecioDeOrdenar()
    {
        Console.WriteLine("PARTE C · EL PRECIO DE ORDENAR (1.000.000 de datos DESORDENADOS)");
        Console.WriteLine("Lineal busca directo. Binaria necesita ordenar primero (una sola vez) y recién después busca.");
        Console.WriteLine();
        int n = 1_000_000;
        var azar = new Random(7);
        var desordenado = new int[n];
        for (int i = 0; i < n; i++) desordenado[i] = azar.Next(0, 10_000_000);

        var claves = new int[10_000];
        for (int i = 0; i < claves.Length; i++) claves[i] = desordenado[azar.Next(0, n)];   // siempre existen

        // calentamiento: que el compilador JIT ya haya compilado todo antes de medir
        Lineal(desordenado, claves[0]);
        var tibio = (int[])desordenado.Clone(); Array.Sort(tibio); Binaria(tibio, claves[0]);

        Console.WriteLine($"{"consultas",10} {"lineal",14} {"ordenar + binaria",20} {"conviene",12}");
        foreach (var k in new[] { 1, 10, 100, 1_000, 10_000 })
        {
            var r = Stopwatch.StartNew();
            long acum = 0;
            for (int i = 0; i < k; i++) acum += Lineal(desordenado, claves[i]).pos;
            r.Stop();
            double tLineal = r.Elapsed.TotalMilliseconds;

            r.Restart();
            var copia = (int[])desordenado.Clone();
            Array.Sort(copia);
            for (int i = 0; i < k; i++) acum += Binaria(copia, claves[i]).pos;
            r.Stop();
            double tOrdenar = r.Elapsed.TotalMilliseconds;
            GC.KeepAlive(acum);

            Console.WriteLine($"{k,10:N0} {tLineal,11:N1} ms {tOrdenar,17:N1} ms {(tLineal < tOrdenar ? "lineal" : "ordenar+bin"),12}");
        }
        Console.WriteLine();
        Console.WriteLine("Pocas consultas: no vale ordenar. Muchas consultas: ordenar UNA vez se paga solo.");
        Console.WriteLine("El punto de cruce depende de TU máquina y de TUS datos: por eso se mide.");
    }
}
