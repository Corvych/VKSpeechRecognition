using System;
using System.Text.Json;
using System.Net;

namespace SR
{
    public class SpeechRecognition
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static string? token = "YOUR TOKEN HERE";
        private static string? servUrl = "";
        public static string? taskid = "";
        public static string? status = "";
        public static string? text = "";
        public static bool? start;
        public static string? req;

        private static void parseResponseServUrl(string response)
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            var resp = root.GetProperty("response");
            servUrl = Convert.ToString(resp.GetProperty("upload_url"));
            start = true;
        }
        private static void parseResponseProcess(string response)
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            var resp = root.GetProperty("response");
            taskid = Convert.ToString(resp.GetProperty("task_id"));
        }
        private static void parseResponseStatus(string response)
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            var resp = root.GetProperty("response");
            status = Convert.ToString(resp.GetProperty("status"));
        }
        private static void parseResponseText(string response)
        {
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            var resp = root.GetProperty("response");
            text = Convert.ToString(resp.GetProperty("text"));
        }
        public static async Task getServUrl()
        {
            string responseString = httpClient.GetStringAsync($"https://api.vk.com/method/asr.getUploadUrl?access_token={token}&v=5.131").Result;
            parseResponseServUrl(responseString);
        }
        public static async Task startSpeechRecog(string request)
        {
            string responseString = httpClient.GetStringAsync($"https://api.vk.com/method/asr.process?access_token={token}&audio={request}&model=spontaneous&v=5.131").Result;
            parseResponseProcess(responseString);
        }
        public static async Task uplToVKServ(string fp)
        {
            start = false;
            if (File.Exists(fp))
            {
                FileStream fs = new FileStream(fp, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                // Generate post objects
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("file", new FormUpload.FileParameter(data, "file", "multipart/form-data"));

                // Create request and receive response
                string postURL = servUrl;
                string userAgent = "Remember Me";
                HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

                // Process response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                req = responseReader.ReadToEnd();
                webResponse.Close();
            }
        }
        public static async Task getTaskStat()
        {
            status = "";
            while (status != "finished" && taskid != "")
            {
                string responseString = httpClient.GetStringAsync($"https://api.vk.com/method/asr.checkStatus?access_token={token}&task_id={taskid}&v=5.131").Result;
                parseResponseStatus(responseString);
                Thread.Sleep(500);
            }
            if (status == "finished" && taskid != "")
            {
                string respString = httpClient.GetStringAsync($"https://api.vk.com/method/asr.checkStatus?access_token={token}&task_id={taskid}&v=5.131").Result;
                parseResponseText(respString);
                Thread.Sleep(500);
            }
        }
    }
}
