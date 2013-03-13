using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ParamBuilder
    {
        public Dictionary<string, object> Params { get; set; } 

        public string[] ParamArray
        {
            get
            {
                return Params == null ? new string[]{} : Params.Keys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(Params[key].ToString()))).ToArray();
            }
        }

        public string QueryString
        {
            get 
            { 
                return ParamArray.Aggregate(string.Empty, (current, param) => current + ((string.IsNullOrWhiteSpace(current) ? string.Empty : "&") + param));
            }
        }

        public ParamBuilder()
        {
            Params = new Dictionary<string, object>();
        }

        public ParamBuilder(Dictionary<string, object> parameters)
        {
            Params = parameters;
        }

        public ParamBuilder(string queryString, bool urlEncoded = true)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return;

            queryString = queryString.Trim();

            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            Params = new Dictionary<string, object>();

            var parameters = queryString.Split('&');
            foreach(var p in parameters)
            {
                var elements = p.Split('=');
                var key = elements[0];
                var value = elements.Length > 0 ? elements[1] : string.Empty;

                if(urlEncoded)
                {
                    key = HttpUtility.UrlDecode(key);
                    value = HttpUtility.UrlDecode(value);
                }

                Params[key] = value;
            }
        }

        public void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if(Params == null)
                Params = new Dictionary<string, object>();

            Params[key] = value ?? string.Empty; 
        }
    }
}
