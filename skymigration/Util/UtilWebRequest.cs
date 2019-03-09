using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skymigration
{
    public enum enumMethod
    {
        GET,
        POST,
        DELETE,
        PUT,
        PATCH,
    }

    public class UtilWebRequest
    {
        public static ResponseOFSC SendWayAsync(string endpoint, enumMethod enumMethod, string data, string token)
        {
            var client = new RestClient("https://api.etadirect.com/" + endpoint);
            RestRequest request = new RestRequest();

            switch (enumMethod.ToString())
            {
                case "PUT":
                    request.Method = Method.PUT;
                    break;

                case "POST":
                    request.Method = Method.POST;
                    break;
                case "PATCH":
                    request.Method = Method.PATCH;
                    break;
                case "GET":
                    request.Method = Method.GET;
                    break;
                case "DELETE":
                    request.Method = Method.DELETE;
                    break;
                default:
                    break;
            }

            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(data))
                request.AddParameter("undefined", data, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            ResponseOFSC responseOFSC = new ResponseOFSC();
            responseOFSC.Content = response.Content;
            responseOFSC.statusCode = (int)response.StatusCode;
            responseOFSC.ErrorMessage = response.ErrorMessage;
            return responseOFSC;
        }

        public static dynamic IsNullOrEmpty(dynamic value)
        {
            if (value == null)
                return null;
            else if (string.IsNullOrEmpty(value))
                return null;
            return value;
        }

    }

    public class ItemEqualityComparer : IEqualityComparer<Activity>
    {
        public bool Equals(Activity x, Activity y)
        {
            return x.apptNumber == y.apptNumber;
        }

        public int GetHashCode(Activity obj)
        {
            return obj.activityId.GetHashCode();
        }
    }


}
