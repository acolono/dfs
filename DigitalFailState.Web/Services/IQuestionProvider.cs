using DigitalFailState.Web.Models;

namespace DigitalFailState.Web.Services {
    public interface IQuestionProvider {
        QuestionModel GetNextQuestion();
        QuestionModel GetQuestionById(int id);
    }
}