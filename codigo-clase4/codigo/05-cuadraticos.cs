// CLASE 4 · ORDENAMIENTOS CUADRÁTICOS — burbuja e inserción, a mano y medidos
// Parte A muestra cada paso sobre 5 números. Parte B cuenta comparaciones (exacto, igual en toda máquina).
// Parte C mide tiempos con el protocolo. Parte D compara mejor caso, caso al azar y peor caso.

using System.Diagnostics;

namespace C4.Cuadraticos;

public static class Demo
{
    public static void Correr()
    {
        ParteA_Trazas();
        ParteB_Contar();
        ParteC_Medir();
        ParteD_Casos();
    }

    // BURBUJA: compara vecinos y los intercambia si están al revés. Si una pasada no cambió nada, termina.
    private static long Burbuja(int[] a, bool traza = false)
    {
        long comparaciones = 0;
        for (int pasada = 1; pasada < a.Length; pasada++)
        {
            bool huboCambio = false;
            for (int i = 0; i < a.Length - pasada; i++)
            {
                comparaciones++;
                if (a[i] > a[i + 1])
                {
                    (a[i], a[i + 1]) = (a[i + 1], a[i]);
                    huboCambio = true;
                }
            }
            if (traza) Console.WriteLine($"  pasada {pasada}: [{string.Join(", ", a)}]");
            if (!huboCambio) break;
        }
        return comparaciones;
    }

    // INSERCIÓN: como ordenar las cartas en la mano. Toma la siguiente y la desliza hasta su lugar.
    private static long Insercion(int[] a, bool traza = false)
    {
        long comparaciones = 0;
        for (int i = 1; i < a.Length; i++)
        {
            int actual = a[i];
            int j = i - 1;
            while (j >= 0)
            {
                comparaciones++;
                if (a[j] > actual) { a[j + 1] = a[j]; j--; }
                else break;
            }
            a[j + 1] = actual;
            if (traza) Console.WriteLine($"  carta {i}: [{string.Join(", ", a)}]");
        }
        return comparaciones;
    }

    private static int[] DatosAlAzar(int n, int semilla)
    {
        var azar = new Random(semilla);
        var d = new int[n];
        for (int i = 0; i < n; i++) d[i] = azar.Next(0, 1_000_000);
        return d;
    }

    private static void ParteA_Trazas()
    {
        Console.WriteLine("PARTE A · A MANO, PASO A PASO (5 números)");
        var original = new[] { 5, 2, 9, 1, 7 };
        Console.WriteLine($"Burbuja sobre [{string.Join(", ", original)}]:");
        var b = (int[])original.Clone();
        var cb = Burbuja(b, traza: true);
        Console.WriteLine($"  comparaciones: {cb}");
        Console.WriteLine();
        Console.WriteLine($"Inserción sobre [{string.Join(", ", original)}]:");
        var s = (int[])original.Clone();
        var ci = Insercion(s, traza: true);
        Console.WriteLine($"  comparaciones: {ci}");
        Console.WriteLine();
    }

    private static void ParteB_Contar()
    {
        Console.WriteLine("PARTE B · CONTAR COMPARACIONES (datos al azar; n x2 → comparaciones x4)");
        Console.WriteLine();
        Console.WriteLine($"{"n",8} {"burbuja",16} {"x",7} {"inserción",16} {"x",7}");
        long antB = 0, antI = 0;
        foreach (var n in new[] { 1_000, 2_000, 4_000, 8_000 })
        {
            var cb = Burbuja(DatosAlAzar(n, 5));
            var ci = Insercion(DatosAlAzar(n, 5));
            string rb = antB > 0 ? $"x{(double)cb / antB:0.0}" : "—";
            string ri = antI > 0 ? $"x{(double)ci / antI:0.0}" : "—";
            Console.WriteLine($"{n,8:N0} {cb,16:N0} {rb,7} {ci,16:N0} {ri,7}");
            antB = cb; antI = ci;
        }
        Console.WriteLine();
        Console.WriteLine("Doble de datos → cuádruple de comparaciones, en los dos. Eso es O(n²).");
        Console.WriteLine();
    }

    private static void ParteC_Medir()
    {
        Console.WriteLine("PARTE C · MEDIR (datos al azar)");
        Console.WriteLine("PROTOCOLO: 1 corrida de calentamiento + 3 corridas medidas + promedio.");
        Console.WriteLine();
        Console.WriteLine($"{"n",8} {"burbuja",14} {"x",7} {"inserción",14} {"x",7}");
        double antB = 0, antI = 0;
        foreach (var n in new[] { 4_000, 8_000, 16_000, 32_000 })
        {
            var datos = DatosAlAzar(n, 9);
            double Medir(Func<int[], bool, long> orden)
            {
                var r = Stopwatch.StartNew();
                orden((int[])datos.Clone(), false);
                r.Stop();
                return r.Elapsed.TotalMilliseconds;
            }
            Func<int[], bool, long> fb = (a, t) => Burbuja(a, t);
            Func<int[], bool, long> fi = (a, t) => Insercion(a, t);
            Medir(fb); Medir(fi);
            double sb = 0, si = 0;
            for (int c = 0; c < 3; c++) { sb += Medir(fb); si += Medir(fi); }
            double pb = sb / 3, pi = si / 3;
            string rb = antB > 0 ? $"x{pb / antB:0.0}" : "—";
            string ri = antI > 0 ? $"x{pi / antI:0.0}" : "—";
            Console.WriteLine($"{n,8:N0} {pb,11:N1} ms {rb,7} {pi,11:N1} ms {ri,7}");
            antB = pb; antI = pi;
        }
        Console.WriteLine();
        Console.WriteLine("Los dos son O(n²): al duplicar n el tiempo sube cerca de x4 (con ruido). La inserción hace la mitad de comparaciones (mirá la parte B) y por eso suele ganar.");
        Console.WriteLine();
    }

    private static void ParteD_Casos()
    {
        Console.WriteLine("PARTE D · MEJOR CASO, AL AZAR, PEOR CASO (n = 8.000, comparaciones)");
        Console.WriteLine();
        int n = 8_000;
        var ordenado = Enumerable.Range(0, n).ToArray();
        var inverso = Enumerable.Range(0, n).Reverse().ToArray();
        var azar = DatosAlAzar(n, 5);
        Console.WriteLine($"{"datos",12} {"burbuja",16} {"inserción",16}");
        foreach (var (nombre, d) in new[] { ("ordenado", ordenado), ("al azar", azar), ("inverso", inverso) })
        {
            var cb = Burbuja((int[])d.Clone());
            var ci = Insercion((int[])d.Clone());
            Console.WriteLine($"{nombre,12} {cb,16:N0} {ci,16:N0}");
        }
        Console.WriteLine();
        Console.WriteLine("Ya ordenado: los dos terminan en ~n pasos (O(n)). Inverso: n²/2. El peor caso manda en la garantía.");
    }
}
