namespace school_management_backend.Models
{
    public class CustomResponse
    {
        public string request_token { get; set; }

        public string message { get; set; }

        public string error { get; set; }
        public int? code { get; set; }
        public int? total { get; set; }
        public dynamic result { get; set; }



    }
}
