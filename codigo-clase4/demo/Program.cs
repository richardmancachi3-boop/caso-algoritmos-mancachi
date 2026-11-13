Console.OutputEncoding = System.Text.Encoding.UTF8;

var caso = args.Length > 0 ? args[0].ToLowerInvariant() : "todo";

switch (caso)
{
    case "busquedas":
        C4.Busquedas.Demo.Correr();
        break;
    case "cuadraticos":
        C4.Cuadraticos.Demo.Correr();
        break;
    case "todo":
        C4.Busquedas.Demo.Correr();
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
        C4.Cuadraticos.Demo.Correr();
        break;
    default:
        Console.WriteLine("Uso: dotnet run -c Release -- [busquedas | cuadraticos | todo]");
        break;
}
