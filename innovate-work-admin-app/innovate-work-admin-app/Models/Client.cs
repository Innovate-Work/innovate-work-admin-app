namespace innovate_work_admin_app.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }

        public bool WithSubscription { get; set; }
        public bool IsCustom { get; set; }
    }
}
