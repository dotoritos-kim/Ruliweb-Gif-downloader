//----------------------------------------
// WebRequest 사용하기 위하여
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
//----------------------------------------

namespace ruliGif
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Program program = new Program();

            string strURL = args[0];

            string[] resourceLink = program.GetHtmlString(strURL);
            foreach (string res in resourceLink)
            {
                var URL = @"http:" + res + "";
                Console.WriteLine(URL);
                await program.DownloadAsync(URL);
            }
        }
        private async Task DownloadAsync(string URL)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL);

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var fileInfo = new FileInfo(Path.GetFileName(URL));
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            Console.WriteLine(Path.GetFileName(URL) + " 다운로드 끝");
        }
        private string[] GetHtmlString(String url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string strHtml = reader.ReadToEnd();

            int RuliStart = strHtml.IndexOf("<div class=\"view_content\">");
            int RuliEnd = strHtml.IndexOf("<div class=\"notice_read_bottom\">") - RuliStart;

            string strSplit = strHtml.Substring(RuliStart, RuliEnd);
            strHtml = strSplit;

            Regex regex = new Regex("//(?:.(?!//))+(\\.gif|\\.mp4)");
            string[] MP4orGIF = regex.Matches(strHtml).Cast<Match>().Select(m => m.Value).ToArray();

            reader.Close();
            response.Close();

            return MP4orGIF;
        }
    }
}
