using EducateApp.Models.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EducateApp.ViewModels.Specialties
{
    public class FilterSpecialtyViewModel
    {
        public string SelectedCode { get; private set; }    // введенный код
        public string SelectedName { get; private set; }    // введенное имя

        public SelectList FormOfStudies { get; private set; } // список форм обучения
        public short? FormOfEdu { get; private set; }   // выбранная форма обучения



        public FilterSpecialtyViewModel(string code, string name,
            List<FormOfStudy> formOfStudies, short? formOfEdu)
        {
            SelectedCode = code;
            SelectedName = name;

            // устанавливаем начальный элемент, который позволит выбрать всех
            formOfStudies.Insert(0, new FormOfStudy { FormOfEdu = "", Id = 0 });

            FormOfStudies = new SelectList(formOfStudies, "Id", "FormOfEdu", formOfEdu);
            FormOfEdu = formOfEdu;
        }
    }
}