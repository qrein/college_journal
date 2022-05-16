using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.ViewModels.Disciplines
{
    public class SortDisciplineViewModel
    {
        public DisciplineSortState CodeSort { get; private set; }
        public DisciplineSortState NameSort { get; private set; }
        public DisciplineSortState Current { get; private set; }     // текущее значение сортировки
    }
}
