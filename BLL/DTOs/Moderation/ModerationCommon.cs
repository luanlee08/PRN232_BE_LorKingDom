namespace BLL.DTOs.Moderation
{
    public class OmniCategories
    {
        public bool Sexual { get; set; }
        public bool SexualMinors { get; set; }
        public bool Harassment { get; set; }
        public bool HarassmentThreatening { get; set; }
        public bool Hate { get; set; }
        public bool HateThreatening { get; set; }
        public bool Illicit { get; set; }
        public bool IllicitViolent { get; set; }
        public bool SelfHarm { get; set; }
        public bool SelfHarmIntent { get; set; }
        public bool SelfHarmInstructions { get; set; }
        public bool Violence { get; set; }
        public bool ViolenceGraphic { get; set; }
    }

    public class OmniCategoryScores
    {
        public decimal Sexual { get; set; }
        public decimal SexualMinors { get; set; }
        public decimal Harassment { get; set; }
        public decimal HarassmentThreatening { get; set; }
        public decimal Hate { get; set; }
        public decimal HateThreatening { get; set; }
        public decimal Illicit { get; set; }
        public decimal IllicitViolent { get; set; }
        public decimal SelfHarm { get; set; }
        public decimal SelfHarmIntent { get; set; }
        public decimal SelfHarmInstructions { get; set; }
        public decimal Violence { get; set; }
        public decimal ViolenceGraphic { get; set; }
    }
}
