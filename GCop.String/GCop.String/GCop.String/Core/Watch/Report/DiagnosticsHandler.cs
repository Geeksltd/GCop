namespace GCop.String.Core.Watch
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Threading.Tasks;

    public sealed class DiagnosticsHandler
    {
        static DiagnosticsHandler _instance;

        internal static DiagnosticsHandler Instance => _instance ?? (_instance = new DiagnosticsHandler());

        internal async Task<string> SendReport(string analyzerName, SyntaxNode nodeToAnalyze, DateTime time, string statue)
        {
            var responseBody = "";
            try
            {
                //if (Settings.Settings.ShouldReportDiagnostics())
                //{
                //    var postData = new List<KeyValuePair<string, string>>();
                //    postData.Add("AnalyzerName", analyzerName);
                //    postData.Add("Statue", statue);
                //    postData.Add("Time", time.ToString());
                //    postData.Add("Source", nodeToAnalyze.GetNodeSourceText());

                //    var content = new FormUrlEncodedContent(postData);

                //    using (var httpClient = new HttpClient())
                //    {
                //        var response = await httpClient.PostAsync(new Uri(Settings.Settings.DiagnosticsURL()), content);
                //        response.EnsureSuccessStatusCode();
                //        responseBody = await response.Content.ReadAsStringAsync();
                //    }
                //}
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return responseBody;
        }

        internal async Task<string> SendError(Exception error, string analyzerName, SyntaxNode nodeToAnalyze, DateTime time)
        {
            var responseBody = "";
            try
            {
                //if (Settings.Settings.ShouldReportDiagnostics())
                //{
                //    var postData = new List<KeyValuePair<string, string>>();
                //    postData.Add("AnalyzerName", analyzerName);
                //    postData.Add("ErrorType", error.GetType().Name);
                //    postData.Add("Error", error.ToFullMessage());
                //    postData.Add("Time", time.ToString());
                //    postData.Add("Source", nodeToAnalyze.GetNodeSourceText());

                //    var content = new FormUrlEncodedContent(postData);

                //    using (var httpClient = new HttpClient())
                //    {
                //        var response = await httpClient.PostAsync(new Uri(Settings.Settings.DiagnosticsURL()), content);
                //        response.EnsureSuccessStatusCode();
                //        responseBody = await response.Content.ReadAsStringAsync();
                //    }
                //}
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return responseBody;
        }
    }
}
