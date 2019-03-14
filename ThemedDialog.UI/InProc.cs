using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.PlatformUI;

namespace ThemedDialog.UI
{
    class InProc
    {
        [STAThread]
        void ShowModal()
        {
            var window = new MainWindow();
            window.ShowModal();
        }

        [STAThread]
        void ExtractStyleFromVisualStudio()
        {
            var dir = @"C:\Source\github.com\jcansdale\ThemedDialog\ThemedDialog.UI\Themes\Blue";
            Directory.CreateDirectory(dir);
            GenerateXamlResources(typeof(CommonControlsColors), "ui", dir);
            GenerateXamlResources(typeof(ThemedDialogColors), "ui", dir);
            GenerateXamlResources(typeof(EnvironmentColors), "ui", dir);
            GenerateXamlResources(typeof(TreeViewColors), "ui", dir);
            GenerateXamlResources(typeof(VsBrushes), "ui", dir);
            GenerateXamlResources(typeof(VsBrushes), "shell", dir);
            GenerateXamlResources(typeof(VsColors), "shell", dir);
        }

        static void GenerateXamlResources(Type keyType, string xmlNamespace, string dir)
        {
            var fileName = $"{keyType.Name}.xaml";
            var file = Path.Combine(dir, fileName);
            using (var writer = new StreamWriter(file))
            {
                writer.WriteLine($"<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
                writer.WriteLine($"                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
                writer.WriteLine($"                    xmlns:{xmlNamespace}=\"clr-namespace:{keyType.Namespace};assembly={keyType.Assembly.GetName().Name}\">");

                var keyProperties = keyType.GetProperties().Where(p => p.Name.EndsWith("Key"));
                var rd = new ResourceDictionary();
                foreach (var keyProperty in keyProperties)
                {
                    var key = keyProperty.GetValue(null);
                    var value = Application.Current.Resources[key];

                    if (value == null)
                    {
                        writer.WriteLine($"<!-- couldn't find value for '{keyProperty.Name}' -->");
                        continue;
                    }

                    if (rd[key] != null)
                    {
                        writer.WriteLine($"<!-- ignoring duplicate key '{key}' -->");
                        continue;
                    }

                    rd.Add(key, value);

                    var xaml = XamlWriter.Save(value);
                    var staticKey = $" x:Key=\"{{x:Static {xmlNamespace}:{keyType.Name}.{keyProperty.Name}}}\"";
                    xaml = xaml.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", staticKey);
                    writer.WriteLine($"    {xaml}");
                }

                writer.WriteLine("</ResourceDictionary>");
            }
        }
    }
}
