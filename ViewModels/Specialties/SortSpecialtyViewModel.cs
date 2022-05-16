namespace EducateApp.ViewModels.Specialties
{
    public class SortSpecialtyViewModel
    {
        public SpecialtySortState CodeSort { get; private set; }
        public SpecialtySortState NameSort { get; private set; }
        public SpecialtySortState FormOfStudySort { get; private set; }
        public SpecialtySortState Current { get; private set; }     // текущее значение сортировки

        public SortSpecialtyViewModel(SpecialtySortState sortOrder)
        {
            CodeSort = sortOrder == SpecialtySortState.CodeAsc ?
                SpecialtySortState.CodeDesc : SpecialtySortState.CodeAsc;

            NameSort = sortOrder == SpecialtySortState.NameAsc ?
                SpecialtySortState.NameDesc : SpecialtySortState.NameAsc;

            FormOfStudySort = sortOrder == SpecialtySortState.FormOfStudyAsc ?
                SpecialtySortState.FormOfStudyDesc : SpecialtySortState.FormOfStudyAsc;
            Current = sortOrder;
        }
    }
}