using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace mobAppTry
{
    class NetWorker
    {
        public string Post(string path, string json)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            return (webClient.UploadString("http://10.0.2.2:4000/api/" + path, json));
        }

        public string Get(string path)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            return (webClient.DownloadString("http://10.0.2.2:4000/api/" + path));
        }
    }
}
