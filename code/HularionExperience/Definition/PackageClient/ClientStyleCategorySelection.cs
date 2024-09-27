using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientStyleCategorySelection
    {

        public string CategoryName { get; set; }

        public string SelectedStyle { get; set; }


        public ClientStyleCategorySelection()
        {
            
        }

        public ClientStyleCategorySelection(StyleCategorySelection selection)
        {
            CategoryName = selection.CategoryName;
            SelectedStyle = selection.SelectedStyle;
        }

    }
}
