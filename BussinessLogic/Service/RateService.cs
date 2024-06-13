using BussinessLogic.DTO;
using DataAccess.Models;
using DataAccess.Repository;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IRateService
    {
        Task<ServiceResponse<bool>> AddRate(int orderId, int rate, string comment);
        Task<ServiceResponse<bool>> DeleteRate(int feedbackId);
    }

    public class RateService : IRateService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public RateService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<ServiceResponse<bool>> AddRate(int orderId, int rate, string comment)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var feedback = new Feedback
                {
                    OrderId = orderId,
                    Rate = rate,
                    Comment = comment,
                    CreateAt = DateTime.UtcNow
                };

                var result = await _feedbackRepository.CreateFeedbackAsync(feedback);

                response.Success = result;
                response.Data = result;
                response.Message = result ? "Feedback added successfully." : "Failed to add feedback.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteRate(int feedbackId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);

                if (feedback != null)
                {
                    var result = await _feedbackRepository.DeleteFeedbackAsync(feedback);

                    response.Success = result;
                    response.Data = result;
                    response.Message = result ? "Feedback deleted successfully." : "Failed to delete feedback.";
                }
                else
                {
                    response.Success = false;
                    response.Data = false;
                    response.Message = "Feedback not found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
