Console.OutputEncoding = System.Text.Encoding.UTF8;

var caso = args.Length > 0 ? args[0].ToLowerInvariant() : "todo";

switch (caso)
{
    case "trazas": C5.Torneo.Demo.Trazas(); break;
    case "contar": C5.Torneo.Demo.Contar(); break;
    case "torneo": C5.Torneo.Demo.Torneo(); break;
    case "peorcaso": C5.Torneo.Demo.PeorCaso(); break;
    case "sincomparar": C5.Torneo.Demo.SinComparar(); break;
    case "todo": C5.Torneo.Demo.Correr(); break;
    default:
        Console.WriteLine("Uso: dotnet run -c Release -- [trazas | contar | torneo | peorcaso | sincomparar | todo]");
        break;
}
