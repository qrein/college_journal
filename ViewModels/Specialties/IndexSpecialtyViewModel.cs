using EducateApp.Models.Data;
using System.Collections.Generic;

namespace EducateApp.ViewModels.Specialties
{
    public class IndexSpecialtyViewModel
    {
        public IEnumerable<Specialty> Specialties { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterSpecialtyViewModel FilterSpecialtyViewModel { get; set; }
        public SortSpecialtyViewModel SortSpecialtyViewModel { get; set; }
    }
}