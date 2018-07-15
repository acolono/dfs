namespace DigitalFailState.Web.Models {
    public class QuestionModel {
        public int Id { get; set; }
        public string Question { get; set; }
        public string YesConclusion { get; set; }
        public string NoConclusion { get; set; }
    }
}