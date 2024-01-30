namespace innovate_work_admin_app.Models.JWT
{
    public class SecurityToken
    {
        public string Token { get; set; }
        //public string UserName { get; set; }
        public DateTime ExpireAt { get; set; } = DateTime.Now.AddMinutes(5);
    }
}
