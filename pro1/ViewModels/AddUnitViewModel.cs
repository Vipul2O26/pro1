using System.Collections.Generic;
using pro1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pro1.ViewModels
{
    public class AddUnitViewModel
    {
        public string UnitName { get; set; }
        public int SelectedSemester { get; set; }
        public int SelectedSubjectID { get; set; }
        public string NewSubject { get; set; } // New property for manual subject entry

        public SelectList Semesters { get; set; }
        public List<SelectListItem> Subjects { get; set; }
    }



}
