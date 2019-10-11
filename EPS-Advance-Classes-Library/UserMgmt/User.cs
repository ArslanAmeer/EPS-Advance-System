using EPS_Advance_Classes_Library.LocationMgmt;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPS_Advance_Classes_Library.UserMgmt
{
    public class User
    {
        public User()
        {
            UserImage = new List<UserImage>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name Is Required")]
        public string FullName { get; set; }

        public string LoginID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ICollection<UserImage> UserImage { get; set; }

        [Required(ErrorMessage = "Full Address required")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Select City/Country")]
        public City CityId { get; set; }

        public virtual Role Role { get; set; }

        public bool IsInRole(int id)
        {
            return Role != null && Role.Id == id;
        }

        public string BirthDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        [Required(ErrorMessage = "Enter Your Phone Number")]
        public long Phone { get; set; }


        public string SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }
    }
}