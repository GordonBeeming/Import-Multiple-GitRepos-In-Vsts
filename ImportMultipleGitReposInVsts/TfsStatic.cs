using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImportMultipleGitReposInVsts
{
    public static class TfsStatic
    {
        #region core

        public const string BOOOOOOOM = "BOOOOOOOM!";
        public static string PatKey = string.Empty;
        public static string BaseUri = string.Empty;

        private static string GetAuthorizationHeader() => $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{PatKey}"))}";

        private static T Get<T>(string uriRelativeToRoot)
        {
            string uri = $@"{BaseUri}{uriRelativeToRoot.Replace("//", "/")}";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = GetAuthorizationHeader();
                return TfsRestTry(uri, () =>
                {
                    var responseString = client.DownloadString(uri);
                    return JsonConvert.DeserializeObject<T>(responseString);
                });
            }
        }

        private static T GeneralPushData<T>(string uriRelativeToRoot, object data, string method, string contentType)
        {
            string uri = $@"{BaseUri}{uriRelativeToRoot.Replace("//", "/")}";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = GetAuthorizationHeader();
                client.Headers[HttpRequestHeader.ContentType] = contentType;
                var requestString = string.Empty;
                if (data != null)
                {
                    requestString = JsonConvert.SerializeObject(data);
                }
                return TfsRestTry(uri, () =>
                {
                    var responseString = client.UploadString(uri, method, requestString);
                    return JsonConvert.DeserializeObject<T>(responseString);
                });
            }
        }

        private static void Post(string uriRelativeToRoot, object data)
        {
            Post<object>(uriRelativeToRoot, data);
        }

        private static T Post<T>(string uriRelativeToRoot, object data)
        {
            return GeneralPushData<T>(uriRelativeToRoot, data, "POST", "application/json");
        }

        private static void Patch(string uriRelativeToRoot, object data)
        {
            Patch<object>(uriRelativeToRoot, data);
        }

        private static T Patch<T>(string uriRelativeToRoot, object data)
        {
            return GeneralPushData<T>(uriRelativeToRoot, data, "PATCH", "application/json");
        }

        private static void Delete(string uriRelativeToRoot, object data)
        {
            Delete<object>(uriRelativeToRoot, data);
        }

        private static T Delete<T>(string uriRelativeToRoot, object data)
        {
            return GeneralPushData<T>(uriRelativeToRoot, data, "DELETE", "application/json");
        }

        private static void Put(string uriRelativeToRoot, object data)
        {
            GeneralPushData<object>(uriRelativeToRoot, data, "PUT", "application/json");
        }

        private static void Patch2(string uriRelativeToRoot, object data)
        {
            Patch2<object>(uriRelativeToRoot, data);
        }

        private static T Patch2<T>(string uriRelativeToRoot, object data)
        {
            return GeneralPushData<T>(uriRelativeToRoot, data, "PATCH", "application/json-patch+json");
        }

        private static T TfsRestTry<T>(string uri, Func<T> f)
        {
            try
            {
                return f();
            }
            catch (WebException webEx) when (webEx.Status == WebExceptionStatus.ProtocolError && (((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.BadRequest || ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.NotFound))
            {
                using (var sr = new StreamReader(webEx.Response.GetResponseStream()))
                {
                    var responseString = sr.ReadToEnd();
                    var exception = JsonConvert.DeserializeObject<RestCallException>(responseString);
                    //throw new Exception($"{exception.message} | {uri}");
                    throw new Exception(exception.message);
                }
            }
        }

        public static bool TryCreate(Action get, Action set)
        {
            return TryCreate(() => { get(); return string.Empty; }, set) != null;
        }

        public static string TryCreate(Func<string> get, Action set)
        {
            // this is bad =)
            try
            {
                Console.Write($"creating...");
                string result = get();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"exists");
                Console.ForegroundColor = ConsoleColor.White;
                return result;
            }
            catch (Exception exThrow)
            {
                bool throwEx = false;
                try
                {
                    set();
                }
                catch (Exception exx) when (exx.Message == TfsStatic.BOOOOOOOM)
                {
                    throwEx = true;
                }
                if (throwEx)
                {
                    throw exThrow;
                }
                try
                {
                    string result = get();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"created");
                    Console.ForegroundColor = ConsoleColor.White;
                    return result;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"err: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    return null;
                }
            }
        }

        #endregion

        public static repositories GetGitRepos(string tpc)
        {
            return Get<repositories>($"/{tpc}/_apis/git/repositories?api-version=1.0");
        }

        public static CreateServiceEndpointResponse CreateServiceEndpoint(string tpc, string tp, CreateServiceEndpointRequest request)
        {
            return Post<CreateServiceEndpointResponse>($"/{tpc}/{tp}/_apis/distributedtask/serviceendpoints?api-version=3.0-preview.1", request);
        }

        public static CreateRepoResponse CreateRepo(string tpc, CreateRepoRequest request)
        {
            return Post<CreateRepoResponse>($"/{tpc}/_apis/git/repositories?api-version=1.0", request);
        }

        public static CreateImportRequestResponse CreateImportRequest(string tpc, string tp, string repoName, CreateImportRequestRequest request)
        {
            return Post<CreateImportRequestResponse>($"/{tpc}/{tp}/_apis/git/repositories/{repoName}/importRequests?api-version=3.0-preview", request);
        }
    }
}
