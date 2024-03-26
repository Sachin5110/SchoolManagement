using System;
using school_management_backend.Models;

namespace school_management_backend.Helpers
{
    public class CommonHelper
    {
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public CustomResponse GenerateApiResponse(string requestToken, int code, string msg, dynamic result)
        {
            var message = "";
            var error = "";
            var reqToken = "";

            if (msg == "" || msg.Split("|").Length <= 1)
            {
                if (code == 200)
                {
                    message = msg;
                    error = "";
                }
                else
                {
                    message = msg;
                    error = msg;
                }

            }
            else
            {
                var messages = msg.Split("|");
                message = messages[0];
                error = messages[1];
            }
            if (!string.IsNullOrEmpty(requestToken))
            {
                reqToken = requestToken;
            }
            var customRes = new CustomResponse();
            customRes.request_token = reqToken;
            customRes.message = message;
            customRes.error = error;
            customRes.result = result;
            customRes.code = code;
            return customRes;
        }
    }
}
