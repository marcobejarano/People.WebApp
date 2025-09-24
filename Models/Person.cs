using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace People.WebApp.Models
{
    public class Person
    {        
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DisplayName("First name")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last name")]
        [Required]
        [StringLength(50, MinimumLength = 2)]        
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Birth date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public int Age
        {
            get
            {
                var dayDiff = (DateTime.Now - BirthDate).TotalDays;

                return Convert.ToInt32(dayDiff / 365);
            }
        }

        public void Update(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException();
            }

            FirstName = person.FirstName;
            LastName = person.LastName;
            BirthDate = person.BirthDate;
        }
    }
}
