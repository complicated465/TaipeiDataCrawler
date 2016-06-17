using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace TaipeiDataCrawler
{
    public class WebCrawer
    {
        private readonly Form1 _form;
        List<Data> allData = new List<Data>();
        public WebCrawer(Form1 form)
        {
            this._form = form;
        }

        List<string> urlList = new List<string>();

        public void craw()
        {
            int count = 0;
            WebClient client = new WebClient();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + @"\metadata_i.txt"))
            {
                StreamReader sr1 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata_name.txt", Encoding.UTF8);
                StreamReader sr2 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata_id.txt", Encoding.UTF8);
                while (!sr1.EndOfStream)
                {
                    Data temp = new Data();
                    temp.Name = sr1.ReadLine();
                    temp.ID = sr2.ReadLine();
                    allData.Add(temp);
                }
            }
            else
            {
                StreamWriter sw1 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\metadata_name.txt", false, Encoding.UTF8);
                StreamWriter sw2 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\metadata_id.txt", false, Encoding.UTF8);

                do
                {
                    MemoryStream ms = new MemoryStream(client.DownloadData("http://data.taipei/opendata/datalist/apiAccess?scope=datasetMetadataSearch&limit=100&offset=0"));

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(ms, Encoding.UTF8);

                    dynamic data = JsonConvert.DeserializeObject(doc.DocumentNode.InnerHtml);
                    foreach (dynamic a in data.result.results)
                    {
                        Data temp = new Data();
                        temp.ID = a.id;
                        //temp.ID.Replace("\"", "");
                        temp.Name = a.title;
                        temp.Name.Replace("\"", "");
                        allData.Add(temp);
                    }
                    //string[] del = { "\"id\":\"", "\",\"title\":\"", "\",\"type\":" };

                   // string[] data = doc.DocumentNode.InnerHtml.Split(del, StringSplitOptions.RemoveEmptyEntries);

                   // string contexnt = doc.DocumentNode.InnerHtml;
                   
                /*    while (contexnt.Contains("\"id\":\""))
                    {
                        Data temp = new Data();
                        int str = 0, end = 0;

                        str = contexnt.IndexOf("\"id\":\"");

                        end = contexnt.IndexOf("\",\"title\":\"");
                        temp.ID = contexnt.Substring(str + 6, end - (str + 6));
                        contexnt = contexnt.Substring(end + 11);
                        end = contexnt.IndexOf("\",\"");
                        temp.Name = contexnt.Substring(0, end);
                        contexnt = contexnt.Substring(end);
                        sw1.WriteLine(temp.Name);
                        sw2.WriteLine(temp.ID);

                    }*/


                    sw1.Close();
                    sw2.Close();
                } while (count >= 0);
               

            }
            //docContext.LoadHtml(doc.DocumentNode.InnerHtml);

            Console.WriteLine("sdfsadf");

           /* int urlIdx = 0;
            while (urlIdx < urlList.Count)
            {
                try
                {
                    string url = urlList[urlIdx];
                    string fileName = "data";// + toFileName(url);
                    urlToFile(url, fileName);
                    String html = fileToText(fileName);
                }
                catch
                {
                    Console.WriteLine("Error:" + urlList[urlIdx] + "fail!");
                }
            }
            urlIdx++;*/
        }


        public static IEnumerable matches(String pPattern, String pText, int pGroupId)
        {
            Regex r = new Regex(pPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            for (Match m = r.Match(pText); m.Success; m = m.NextMatch())
                yield return m.Groups[pGroupId].Value;
        }

        public static String fileToText(String filePath)
        {
            StreamReader file = new StreamReader(filePath);
            String text = file.ReadToEnd();
            file.Close();
            return text;
        }

        public void urlToFile(String url, String file)
        {
            WebClient webclient = new WebClient();
            //        webclient.Proxy = proxy;
            webclient.DownloadFile(url, file);
        }

        public static String toFileName(String url)
        {
            String fileName = url.Replace('?', '_');
            fileName = fileName.Replace('/', '_');
            fileName = fileName.Replace('&', '_');
            fileName = fileName.Replace(':', '_');
            fileName = fileName.ToLower();
            if (!fileName.EndsWith(".htm") && !fileName.EndsWith(".html"))
                fileName = fileName + ".htm";
            return fileName;
        }
    }

}
