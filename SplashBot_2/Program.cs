namespace SplashBot_2
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            App application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}
