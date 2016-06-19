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
            int offset = 0;
            int limit = 100;
            int first = 0;
            WebClient client = new WebClient();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + @"\metadata_.txt"))
            {
                StreamReader sr1 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata.txt", Encoding.UTF8);
               // StreamReader sr2 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata_id.txt", Encoding.UTF8);
                while (!sr1.EndOfStream)
                {
                    Data temp = new Data();
                    string[] data = sr1.ReadLine().Split(new string[] { "\t\t" }, StringSplitOptions.RemoveEmptyEntries);
                    temp.Name = data[0];
                    temp.ID = data[1];
                    allData.Add(temp);
                }
            }
            else
            {
                StreamWriter sw1 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\metadata.txt", false, Encoding.UTF8);
                StreamWriter sw2 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\metadata_id.txt", false, Encoding.UTF8);

                do
                {
                    MemoryStream ms = new MemoryStream(client.DownloadData(string.Format("http://data.taipei/opendata/datalist/apiAccess?scope=datasetMetadataSearch&limit={0}&offset={1}", limit, offset)));

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(ms, Encoding.UTF8);

                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(doc.DocumentNode.InnerHtml);

                        count = data.result.count;
                        foreach (dynamic a in data.result.results)
                        {
                            Data temp = new Data();
                            temp.ID = a.id;
                            //temp.ID.Replace("\"", "");
                            temp.Name = a.title;

                            try
                            {
                                if(a.resources.Count!=0)
                                    if(a.resources[0].GetType().GetProperty("resourceId") != null)
                                        temp.RID = a.resources[0].resourceId;
                            }
                            catch (Exception e)
                            {
                                temp.RID = a.resources.resourceId;
                            }
                            
                            //temp.Name.Replace("\"", "");
                            allData.Add(temp);
                            if (first == 0)
                            {
                                sw1.WriteLine(a.fieldDescription);
                                first++;
                            }
                            sw1.WriteLine(temp.Name + "\t\t" + temp.ID+ "\t\t" + temp.RID);
                            offset++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error:" + e.Message);

                        //string[] del = { "\"id\":\"", "\",\"title\":\"", "\",\"type\":" };

                        //string[] data = doc.DocumentNode.InnerHtml.Split(del, StringSplitOptions.RemoveEmptyEntries);

                        string contexnt = doc.DocumentNode.InnerHtml;

                        while (contexnt.Contains("\"id\":\""))
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
                            str = contexnt.IndexOf("\"resourceId\":\"");
                            contexnt = contexnt.Substring(str+14);
                            end = contexnt.IndexOf("\",\"");
                            temp.RID = contexnt.Substring(0, end);
                            contexnt = contexnt.Substring(end);
                            allData.Add(temp);
                            sw1.WriteLine(temp.Name + "\t\t" + temp.ID + "\t\t" + temp.RID);
                            offset++;

                        }
                    }


                } while ((count-offset) > 0);
                sw1.Close();
                sw2.Close();

            }
            

            Console.WriteLine("sdfsadf");

           // _form.update("ffff");
            _form.update(allData);
        }

        public void craw(string[] url)
        {
            int count = 0;
            int offset = 0;
            int limit = 100;
            WebClient client = new WebClient();

            StreamWriter sw1 = new StreamWriter(string.Format(System.Windows.Forms.Application.StartupPath + @"\{0}.txt",url[0]), false, Encoding.UTF8);
     
            do
            {
                MemoryStream ms = new MemoryStream(client.DownloadData(string.Format("http://data.taipei/opendata/datalist/apiAccess?scope=resourceAquire&rid={0}&limit={1}&offset={2}",url[1], limit, offset)));

                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.UTF8);

                try
                {
                    dynamic data = JsonConvert.DeserializeObject(doc.DocumentNode.InnerHtml);
                    count = data.result.count;

                    sw1.Write(doc.DocumentNode.InnerHtml);
                    offset = offset + limit;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:" + e.Message);
                    sw1.Write(doc.DocumentNode.InnerHtml);
                    offset = offset + limit;

                }


            } while ((count - offset) > 0);
            sw1.Close();
            Console.WriteLine("sdfsadf");
        }

    }

}
