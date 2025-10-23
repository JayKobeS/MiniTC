using System;
using System.Windows.Forms;
using MiniTC.Model;
using MiniTC.Presenter;

namespace MiniTC
{
    static class Projekt 
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var view = new Okno();
            var model = new Model.Model();
            var presenter = new Presenter.Presenter(view, model);

            Application.Run(view);
        }
    }
}