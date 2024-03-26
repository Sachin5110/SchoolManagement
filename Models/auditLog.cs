using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace school_management_backend.Models
{
    public class auditlog
    {
        [Key]
        public long auditlog_id { get; set; }
        public string? auditlog_request_token { get; set; }
        public string? auditlog_request_url { get; set; }
        public string? auditlog_request_method { get; set; }
        public string? auditlog_request_query { get; set; }
        public string? auditlog_request_payload { get; set; }
        public string? auditlog_response_payload { get; set; }
        public string? auditlog_ip_address { get; set; }
        public string? auditlog_browser_useragent { get; set; }
        public DateTime? auditlog_event_datetime { get; set; }
        public int? auditlog_event_actor { get; set; }
        public Guid auditlog_event_actor_id { get; set; }
        public byte? auditlog_authorized { get; set; }
        public int? auditlog_response_code { get; set; }
        public DateTime? auditlog_created { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public IConfiguration Configuration { get; }
        public int SaveDetails(dynamic audit, string date)
        {
            if (audit.auditlog_response_payload != null && audit.auditlog_response_payload.Contains("'"))
            {
                audit.auditlog_response_payload = audit.auditlog_response_payload.Replace("'","\"");
            }
                string constring = GetConnectionStringFromEnv();
                SqlConnection con = new SqlConnection(constring);
                string query = $"INSERT INTO auditlog (auditlog_request_token, auditlog_request_url, auditlog_request_method, auditlog_request_query, auditlog_request_payload, auditlog_response_payload, auditlog_ip_address, auditlog_browser_useragent, auditlog_event_datetime, auditlog_event_actor, auditlog_event_actor_id, auditlog_authorized, auditlog_response_code, auditlog_created, created_at, updated_at) values ('{audit.auditlog_request_token}','{audit.auditlog_request_url}','{audit.auditlog_request_method}','{audit.auditlog_request_query}','{audit.auditlog_request_payload}','{audit.auditlog_response_payload}','{audit.auditlog_ip_address}','{audit.auditlog_browser_useragent}','{date}','{audit.auditlog_event_actor}','{audit.auditlog_event_actor_id}','{audit.auditlog_authorized}','{audit.auditlog_response_code}','{date}','{date}','{date}')";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                int res = cmd.ExecuteNonQuery();
                con.Close();
                return res;            
        }
        private string GetConnectionStringFromEnv()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            return connectionString;
        }
    }
}
