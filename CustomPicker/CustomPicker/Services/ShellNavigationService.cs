using CustomPicker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPicker.Services
{
    public class ShellNavigationService : INavigationService
    {
        public Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
        {
            if (parameters != null)
                return Shell.Current.GoToAsync(route, parameters);
            return Shell.Current.GoToAsync(route);
        }

        public Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}
