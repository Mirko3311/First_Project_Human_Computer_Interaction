using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace PrviProjektniZadatakHCI
{
    public partial class App : Application
    {
        
        public App()
        {
            SetLanguage("sr"); 
        }

      
        public void SetLanguage(string languageCode)
        {
           
            CultureInfo ci = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

           
            ResourceManager resourceManager = new ResourceManager("PrviProjektniZadatakHCI.Resources.SharedResource", typeof(App).Assembly);

       
            this.Resources.Clear();
            this.Resources.Add("SharedResource", resourceManager);
        }

        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri };

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);

        }
    }
}