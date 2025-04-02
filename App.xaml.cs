using ASystem;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Markup;

namespace PrviProjektniZadatakHCI
{
    public partial class App : Application
    {
        
        public App()
        {
           // SetLanguage("sr"); 
        }


        /*  public void SetLanguage(string languageCode)
          {

              CultureInfo ci = new CultureInfo(languageCode);
              Thread.CurrentThread.CurrentCulture = ci;
              Thread.CurrentThread.CurrentUICulture = ci;


              ResourceManager resourceManager = new ResourceManager("PrviProjektniZadatakHCI.Resources.SharedResource", typeof(App).Assembly);


              this.Resources.Clear();
              this.Resources.Add("SharedResource", resourceManager);
          }*/

        public void ChangeLanguage(string languageCode)
        {
            var cultureInfo = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            ResourceDictionary languageResource = new ResourceDictionary();
            if (languageCode == "sr")
            {
                languageResource.Source = new Uri("pack://application:,,,/PrviProjektniZadatakHCI;component/Resources/SerbianLanguage.xaml");
            }
            else
            {
                languageResource.Source = new Uri("pack://application:,,,/PrviProjektniZadatakHCI;component/Resources/EnglishLanguage.xaml");
            }

            // 📌 Sačuvajte postojeće resurse osim jezika
            var oldResources = new List<ResourceDictionary>();
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                if (!dict.Source.ToString().Contains("SerbianLanguage.xaml") &&
                    !dict.Source.ToString().Contains("EnglishLanguage.xaml"))
                {
                    oldResources.Add(dict);
                }
            }

            // 📌 Očistite i dodajte sačuvane resurse + novi jezički resurs
            Application.Current.Resources.MergedDictionaries.Clear();
            foreach (var dict in oldResources)
            {
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            Application.Current.Resources.MergedDictionaries.Add(languageResource);

            foreach (Window window in Application.Current.Windows)
            {
                if (window is ProfessorWindow professorWindow)
                {
                    professorWindow.RefreshAllDataGrids();
                }
            }
        }

        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri };

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);

        }

 
     
    }
}