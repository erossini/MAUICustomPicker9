using CustomPicker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPicker.Components
{
    public class LanguageDropDown : DropDownButton<LanguageModel>
    {
        public LanguageDropDown()
        {
            DisplayMember = nameof(LanguageModel.Name);
        }
    }
}