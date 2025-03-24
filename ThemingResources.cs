using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrviProjektniZadatakHCI
{
    public static class ThemingResources
    {
        public static readonly Uri SquaredThemeResourcesFileUri = new Uri("pack://application:,,,/PrviProjektniZadatakHCI;component/Resources/BlueTheme.xaml");
        public static readonly Uri RoundThemeResourcesFileUri = new Uri("pack://application:,,,/PrviProjektniZadatakHCI;component/Resources/RedTheme.xaml");

        public static ComponentResourceKey ButtonBackgroundBrushKey = new ComponentResourceKey(typeof(ThemingResources), "ButtonBackgroundBrushKey");
        public static ComponentResourceKey ButtonBackgroundColorKey = new ComponentResourceKey(typeof(ThemingResources), "ButtonBackgroundColorKey");
        public static ComponentResourceKey ButtonCornerRadiusKey = new ComponentResourceKey(typeof(ThemingResources), "ButtonCornerRadiusKey");
    }
}
