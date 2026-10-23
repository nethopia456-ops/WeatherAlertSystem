
namespace WeatherAlertSystem;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Velopack.VelopackApp.Build().Run();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
