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
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.VisualBasic.FileIO;

namespace TaipeiDataCrawler
{
   
    public class WebCrawer
    {
       
        private readonly Form1 _form;
        List<Data> allData = new List<Data>();
        List<Data> weatherData = new List<Data>();
        List<govData> govDataList = new List<govData>();
        public WebCrawer(Form1 form)
        {
            this._form = form;
        }

        List<string> urlList = new List<string>();

        //data.taipe 目錄抓取
        public void craw()
        {
            int count = 0;
            int offset = 0;   //跳過x筆資料
            int limit = 100;  //回傳數量限制
            int first = 0;
            WebClient client = new WebClient();

            if (File.Exists(System.Windows.Forms.Application.StartupPath + @"\metadata.txt"))
            {
                StreamReader sr1 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata.txt", Encoding.UTF8);
                while (!sr1.EndOfStream)
                {
                    Data temp = new Data();
                    string[] data = sr1.ReadLine().Split(new string[] { "\t\t" }, StringSplitOptions.RemoveEmptyEntries);
                    temp.Name = data[0];
                    temp.ID = data[1];
                    if (data.Length == 3)
                        temp.RID = data[2];
                    allData.Add(temp);
                }
            }
            else
            {
                StreamWriter sw1 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\metadata.txt", false, Encoding.UTF8);
             
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
                            temp.Name = a.title;

                            try
                            {
                                if (a.resources.Count != 0)
                                {
                                    
                                    if (a.resources[0].resourceId !=null)
                                        temp.RID = a.resources[0].resourceId;
                                }
                            }
                            catch (Exception e)
                            {
                                temp.RID = a.resources.resourceId;
                            }
                                                      
                            allData.Add(temp);
                            if (first == 0)
                            {                              
                                first++;
                            }
                            sw1.WriteLine(temp.Name + "\t\t" + temp.ID+ "\t\t" + temp.RID);
                            offset++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error:" + e.Message);

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
                            str = end;
                            if (contexnt.IndexOf("\"id\":\"") != -1)
                            {
                                end = contexnt.IndexOf("\"id\":\"");
                                string res = contexnt.Substring(str, end - str);
                                if (res.Contains("\"resourceId\":\""))
                                {
                                    str = contexnt.IndexOf("\"resourceId\":\"");
                                    contexnt = contexnt.Substring(str + 14);
                                    end = contexnt.IndexOf("\",\"");
                                    temp.RID = contexnt.Substring(0, end);
                                    contexnt = contexnt.Substring(end);
                                }
                            }
                            allData.Add(temp);
                            sw1.WriteLine(temp.Name + "\t\t" + temp.ID + "\t\t" + temp.RID);
                            offset++;

                        }
                    }


                } while ((count-offset) > 0);
                sw1.Close();
               

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
                HtmlDocument doc = new HtmlDocument();
                
                try
                {
                    MemoryStream ms = new MemoryStream(client.DownloadData(string.Format("http://data.taipei/opendata/datalist/apiAccess?scope=resourceAquire&rid={0}&limit={1}&offset={2}", url[1], limit, offset)));             
                    doc.Load(ms, Encoding.UTF8);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("無法下載!!");
                    sw1.Close();
                    return;
                }
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
            System.Windows.Forms.MessageBox.Show("下載完成!!");
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            return settings.GetType().GetProperty(name) != null;
        }

        public void weatherCraw()
        {
            WebClient client = new WebClient();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + @"\weathercontent.txt"))
            {
                StreamReader sr1 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\weathercontent.txt", Encoding.UTF8);
                // StreamReader sr2 = new StreamReader(System.Windows.Forms.Application.StartupPath + @"\metadata_id.txt", Encoding.UTF8);
                while (!sr1.EndOfStream)
                {
                    Data temp = new Data();
                    string[] data = sr1.ReadLine().Split(new string[] { "\t\t"," ","\t","  " }, StringSplitOptions.RemoveEmptyEntries);
                    temp.weatherContent = data[0];
                    temp.weatherKey = data[1];

                    weatherData.Add(temp);
                }
                sr1.Close();
            }
            

            _form.updateWeather(weatherData);
        }

        public void weatherCraw(string[] url)
        {
            string auKey = "CWB-18E287BF-DF2C-4019-BA5B-41D58E91C750";
            WebClient client = new WebClient();
            HttpClient client1 = new HttpClient();

            HtmlDocument doc = new HtmlDocument();
            try
            {
               
                client.DownloadFile(string.Format("http://opendata.cwb.gov.tw/opendataapi?dataid={0}&authorizationkey={1}", url[1], auKey), string.Format(System.Windows.Forms.Application.StartupPath + @"\{0}.txt", url[0]));
               
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show("無法下載!!");
               
                return;
            }

            
            
            Console.WriteLine("sdfsadf");
            System.Windows.Forms.MessageBox.Show("下載完成!!");
        }

        public async void govCraw()
        {
            WebClient client = new WebClient();
            HttpClient client1 = new HttpClient();
           /**   update dataset**/ 
           /*   using (WebClient client2 = new WebClient())
            {
                client2.DownloadFile(@"http://file.data.gov.tw/opendatafile/政府資料開放平臺資料集清單.csv",
                                   System.Windows.Forms.Application.StartupPath + @"\政府資料開放平臺資料集清單.csv");
            }*/
            if (File.Exists(System.Windows.Forms.Application.StartupPath + @"\政府資料開放平臺資料集清單.csv"))
            {
                TextFieldParser parser = new TextFieldParser((System.Windows.Forms.Application.StartupPath + @"\政府資料開放平臺資料集清單.csv"));
                
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] nowFields = parser.ReadFields();
                nowFields = parser.ReadFields();
                while (!parser.EndOfData)
                {
       
                    //Processing row
                    govData temp = new govData();
                    temp.tite = nowFields[0];
                    temp.distrbution = new List<resourceData>();
                    resourceData res = new resourceData();
                    res.format = nowFields[1];
                    res.downloadURL = nowFields[2];
                    temp.distrbution.Add(res);

                    
                    string[] nextFields = parser.ReadFields();
                    if (parser.EndOfData)
                    {
                        break;
                    }                     

                    //檢查是否有一樣
                    while (nextFields[0] == nowFields[0])
                    {
                            
                        res = new resourceData();
                        res.format = nextFields[1];
                        res.downloadURL = nextFields[2];
                        temp.distrbution.Add(res);
                        nextFields = parser.ReadFields();
                        if (parser.EndOfData)
                        {
                            break;
                        }
                    
                    }
                    govDataList.Add(temp);
                    nowFields = nextFields;

                    
                }

                
            }
            else
            {
                return;
               /** update dataset(xml)*/
               /* StreamWriter sw1 = new StreamWriter(System.Windows.Forms.Application.StartupPath + @"\govContent.txt",false, Encoding.UTF8);
                HttpResponseMessage response = await client1.GetAsync(("http://file.data.gov.tw/opendatafile/政府資料開放平臺資料集清單.xml"));
                MemoryStream ms1 = new MemoryStream(client.DownloadData("http://file.data.gov.tw/opendatafile/政府資料開放平臺資料集清單.xml"));

                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms1, Encoding.UTF8);



                HtmlDocument docStockContext = new HtmlDocument();
                docStockContext.LoadHtml(doc.DocumentNode.SelectSingleNode("/records[1]").InnerHtml);

                HtmlNodeCollection nodes = docStockContext.DocumentNode.SelectNodes("/record");

                for (int i = 0; i < nodes.Count; i++)
                {
                    govData temp = new govData();

                    temp.tite = nodes[i].SelectSingleNode("資料集名稱").InnerText;

                    temp.distrbution = new List<resourceData>();
                    resourceData res = new resourceData();
                    res.downloadURL = nodes[i].SelectSingleNode("下載連結").InnerText;
                    res.format = nodes[i].SelectSingleNode("檔案格式").InnerText;

                    sw1.Write(temp.tite + "\t\t" + res.downloadURL + "\t\t" + res.format);
                    temp.distrbution.Add(res);
                    if (i == nodes.Count - 1) break;
                    while (nodes[i].SelectSingleNode("資料集名稱").InnerText == nodes[i + 1].SelectSingleNode("資料集名稱").InnerText)
                    {
                        res.downloadURL = nodes[i + 1].SelectSingleNode("下載連結").InnerText;
                        res.format = nodes[i + 1].SelectSingleNode("檔案格式").InnerText;
                        sw1.Write("\t\t" + res.downloadURL + "\t\t" + res.format);
                        temp.distrbution.Add(res);
                        i++;
                        if (i == nodes.Count - 1) break;
                    }
                    sw1.Write("\n");
                    govDataList.Add(temp);

                }*/

                /* dynamic data = JsonConvert.DeserializeObject(doc.DocumentNode.InnerHtml);

                 dynamic a = data.Records;
                 for (int i = 0; i < data.Records.Count; i++)
                 {


                     govData temp = new govData();


                     temp.tite = a[i].資料集名稱;
                     temp.distrbution = new List<resourceData>();
                     resourceData res = new resourceData();
                     res.downloadURL = a[i].下載連結;
                     res.format = a[i].檔案格式;
                     temp.distrbution.Add(res);
                     if (i == data.Records.Count - 1) break;
                     while (a[i].資料集名稱==a[i+1].資料集名稱)
                     {
                         res.downloadURL = a[i+1].下載連結;
                         res.format = a[i+1].檔案格式;
                         temp.distrbution.Add(res);
                         i++;
                         if (i == data.Records.Count - 1) break;
                     }
                     govDataList.Add(temp);
                 }*/

                //sw1.Close();


            }


            _form.updateGov(govDataList);
        }
        public async void govCraw(string[] url)
        {
            WebClient client = new WebClient();           
            HttpClient client1 = new HttpClient();                 
            HtmlDocument doc = new HtmlDocument();
          
            try
            {
                var fi = new Uri(url[1]);
                var fii = new FileInfo(fi.AbsolutePath);
                HttpResponseMessage response = await client1.GetAsync((url[1]));
                string ext = "";
                if (response.Content.Headers.ContentDisposition != null)
                {
                    string[] dis = response.Content.Headers.ContentDisposition.FileName.Split('.');
                    ext = dis[1];
                }
                else 
                {
                    ext = Path.GetExtension(response.RequestMessage.RequestUri.LocalPath.Split(';')[0]);
                }
               
                string[] format1 = (response.Content.Headers.ContentType.MediaType).Split('/');
                ext.Trim('/','"');
                
                client.DownloadFile(url[1], string.Format(System.Windows.Forms.Application.StartupPath + @"\{0}{1}", url[0], ext) );
               
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("無法下載!!");             
                return;
            }
         
            Console.WriteLine("sdfsadf");
            System.Windows.Forms.MessageBox.Show("下載完成!!");

            client1.Dispose();
            client.Dispose();

        }

    }

}
