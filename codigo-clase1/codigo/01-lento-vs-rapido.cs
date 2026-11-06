// CLASE 1 - EXPERIMENTO DE BUSQUEDA LINEAL
// Objetivo: comprobar el crecimiento del tiempo
// utilizando n, 4n y 16n elementos.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace C1.Duelo;

public static class Demo
{
    // BUSQUEDA LINEAL
    public static int BusquedaLineal(
        List<int> datos, int cantidad, int objetivo)
    {
        for (int i = 0; i < cantidad; i++)
        {
            if (datos[i] == objetivo)
                return i;
        }

        return -1;
    }

    // MEDIR UNA CORRIDA
    static double MedirCorrida(
        List<int> datos,
        int cantidad,
        int repeticiones,
        out long control)
    {
        control = 0;

        var reloj = Stopwatch.StartNew();

        for (int i = 0; i < repeticiones; i++)
        {
            int resultado = BusquedaLineal(datos, cantidad, -1);

            if (resultado == -1)
                control++;
        }

        reloj.Stop();

        return reloj.Elapsed.TotalMilliseconds;
    }

    public static void Correr()
    {
        // 1. CONFIGURACION
        const int n = 62_500;
        const int corridas = 5;
        const double tiempoObjetivo = 5.0;

        int tamanoPadron = 16 * n;

        // 2. GENERAR EL PADRON
        var padronLista = new List<int>(tamanoPadron);

        for (int i = 0; i < tamanoPadron; i++)
        {
            padronLista.Add(1_000_000 + i * 3);
        }

        int[] tamanos = { n, 4 * n, 16 * n };

        Console.WriteLine("EXPERIMENTO: BUSQUEDA LINEAL");
        Console.WriteLine("--------------------------------");

        Console.WriteLine(
            $"Padron total: {tamanoPadron:N0} registros");

        Console.WriteLine();

        // 3. CALENTAMIENTO
        Console.WriteLine("Calentando el programa...");

        foreach (int tam in tamanos)
        {
            for (int i = 0; i < 3; i++)
            {
                BusquedaLineal(padronLista, tam, -1);
            }
        }

        // 4. CALIBRAR REPETICIONES
        int repeticiones = 1;

        while (true)
        {
            double prueba = MedirCorrida(
                padronLista, n, repeticiones, out _);

            if (prueba >= tiempoObjetivo)
                break;

            repeticiones *= 2;
        }

        Console.WriteLine(
            $"Repeticiones por corrida: {repeticiones}");

        Console.WriteLine(
            "Objetivo buscado: -1 (no existe en el padron)");

        Console.WriteLine();

        // 5. CINCO CORRIDAS Y PROMEDIOS
        double[] promedios = new double[3];

        long controlTotal = 0;

        Console.WriteLine("TABLA DE RESULTADOS");
        Console.WriteLine();

        Console.WriteLine(
            "Tamaño".PadLeft(12) +
            "C1 (ms)".PadLeft(12) +
            "C2 (ms)".PadLeft(12) +
            "C3 (ms)".PadLeft(12) +
            "C4 (ms)".PadLeft(12) +
            "C5 (ms)".PadLeft(12) +
            "Promedio".PadLeft(12) +
            "Ratio".PadLeft(10)
        );

        for (int k = 0; k < tamanos.Length; k++)
        {
            double suma = 0;

            Console.Write($"{tamanos[k],12:N0}");

            for (int c = 0; c < corridas; c++)
            {
                double tiempo = MedirCorrida(
                    padronLista,
                    tamanos[k],
                    repeticiones,
                    out long control
                );

                suma += tiempo;
                controlTotal += control;

                Console.Write($"{tiempo,12:F2}");
            }

            promedios[k] = suma / corridas;

            Console.Write($"{promedios[k],12:F2}");

            if (k == 0)
            {
                Console.WriteLine("—".PadLeft(10));
            }
            else
            {
                double ratio =
                    promedios[k] / promedios[k - 1];

                Console.WriteLine($"{ratio,9:F2}x");
            }
        }

        // 6. COMPARACION DE RATIOS
        Console.WriteLine();

        Console.WriteLine("COMPARACION DE RATIOS");

        Console.WriteLine(
            $"Ratio 4n / n: {promedios[1] / promedios[0]:F2}x");

        Console.WriteLine(
            $"Ratio 16n / 4n: {promedios[2] / promedios[1]:F2}x");

        Console.WriteLine(
            $"Ratio 16n / n: {promedios[2] / promedios[0]:F2}x");

        Console.WriteLine();

        Console.WriteLine(
            $"Busquedas completas: {controlTotal:N0}");

        Console.WriteLine();

        Console.WriteLine("EXPERIMENTO FINALIZADO");
    }
}