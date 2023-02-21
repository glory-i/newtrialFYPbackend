namespace newtrialFYPbackend.Responses
{
    public class EmailValidationResponse
    {
        public string address { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public bool free_email { get; set; }
        public object did_you_mean { get; set; }
        public string account { get; set; }
        public string domain { get; set; }
        public string domain_age_days { get; set; }
        public string smtp_provider { get; set; }
        public string mx_found { get; set; }
        public object mx_record { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
        public object country { get; set; }
        public object region { get; set; }
        public object city { get; set; }
        public object zipcode { get; set; }
        public string processed_at { get; set; }
    }

}
