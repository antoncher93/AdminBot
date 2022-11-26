namespace AdminBot.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var app = ApplicationRoot.BuildApp(args);
        app.Run();
    }
}