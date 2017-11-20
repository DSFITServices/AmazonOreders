using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using MWSClientCsRuntime;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using amazonOrders;

namespace amazonOrders
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonOrderProcessing Amz = new AmazonOrderProcessing("AKIAIMSOQ3ULEO44P2GA", "Eu09VI5yylj6BLWwnMxkJhQjUoVjfMr+hKTT+nzE", "https://mws.amazonservices.co.uk", "MyApp", "0.01a", "C#", "A10HS0L7N8OCDE");
            
            Console.WriteLine(Amz.AppName);

            DateTime startDate = new DateTime(2017, 10, 10, 00, 00, 00);
            DateTime endDate = DateTime.Now;

           Amz.GetRequestOrderResult(startDate, endDate, AmazonOrderProcessing.ReportType._GET_FLAT_FILE_ORDERS_DATA_);

            Console.WriteLine("Amazon Order Request sent");

            

            List<ReportRequestInfo> ReportInfo = new List<ReportRequestInfo>();
            do
            {
                //Thread.Sleep(1 * 60000);
                ReportInfo = Amz.GetReportRequests();
                if (ReportInfo[0].ReportProcessingStatus != "_DONE_")
                {
                    Console.WriteLine("Waiting For Report to be Processed.");
                    Thread.Sleep(2 * 60000);
                }
            } while (ReportInfo[0].ReportProcessingStatus != "_DONE_");



            FileInfo fileInfo = Amz.GetReportFile(ReportInfo[0]);

            Console.WriteLine("Safe to Exit");
            Console.ReadLine();
        }

        
    }
}
