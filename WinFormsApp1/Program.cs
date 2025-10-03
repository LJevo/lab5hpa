namespace WinFormsApp1
{
    //Jose Luis Silvera 8-1013-1016
    //Lenn Mendoza 8-1021-359
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Buscaminas());
        }
    }
}