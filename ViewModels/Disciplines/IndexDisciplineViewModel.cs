using EducateApp.Models.Data;
using System.Collections.Generic;

namespace EducateApp.ViewModels.Disciplines
{
    public class IndexDisciplineViewModel
    {
        public IEnumerable<Discipline> Desciplines { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterDisciplineViewModel FilterSpecialtyViewModel { get; set; }
        public SortDisciplineViewModel SortSpecialtyViewModel { get; set; }
    }
}
