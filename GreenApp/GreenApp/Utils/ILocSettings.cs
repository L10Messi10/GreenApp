using System;
using System.Collections.Generic;
using System.Text;


[assembly: Xamarin.Forms.Dependency(typeof(GreenApp.Utils.ILocSettings))]
namespace GreenApp.Utils
{
    public interface ILocSettings
    {
        void OpenSettings();

    }
}
