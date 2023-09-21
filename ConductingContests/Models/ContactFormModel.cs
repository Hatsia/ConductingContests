using System.ComponentModel.DataAnnotations;

namespace ConductingContests.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите ваше имя.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите адрес электронной почты.")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите сообщение.")]
        public string Message { get; set; }
    }

}
