namespace CMS.Areas.Admin.ViewModels.Personal
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public int Sex { get; set; }
        public string FullName { get; set; }

        public string Image { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public bool HasAuthenticator { get; set; }
    }
}