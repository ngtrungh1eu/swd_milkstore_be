using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Promotion
{
    public class PromotionDTO : IValidatableObject
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public DateTime StartAt { get; set; } = DateTime.Now;
        public DateTime EndAt { get; set; } = DateTime.Now.AddDays(1);
        public bool Status => StartAt <= DateTime.UtcNow && DateTime.UtcNow <= EndAt;

        [Range(0, 100, ErrorMessage = "Promotion value must be between 0 and 100.")]
        public int Promote { get; set; }
        public string? PromotionImg { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndAt < StartAt)
            {
                yield return new ValidationResult(
                    "The end time cannot be less than the start time.",
                    new[] { nameof(EndAt) }
                );
            }
        }
    }
}
