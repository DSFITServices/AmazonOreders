using System;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MWSClientCsRuntime;
using MarketplaceWebService;
using MarketplaceWebService.Attributes;
using MarketplaceWebService.Model;


namespace amazonOrders
{
    class AmazonOrderProcessing
    {
        private string mwsAccessKey;
        private string mwsSecretAccessKey;
        private string mwsServiceUrl;
        private string mwsAppName;
        private string mwsAppVersion;
        private string mwsCode;
        private string mwsMerchant;
        private MarketplaceWebServiceConfig mwsConfig;
        private ReportType reportType;
        private List<object> amzOrders;
        private FileInfo fileInfo;
        


        public string AccessKey { get => mwsAccessKey; private set => mwsAccessKey = value; }
        public string SecretAccessKey { get => mwsSecretAccessKey; private set => mwsSecretAccessKey = value; }
        public string ServiceUrl { get => mwsServiceUrl; private set => mwsServiceUrl = value; }
        public string AppName { get => mwsAppName; set => mwsAppName = value; }
        public string AppVersion { get => mwsAppVersion; private set => mwsAppVersion = value; }
        public string Code { get => mwsCode; private set => mwsCode = value; }
        public string MwsMerchant { get => mwsMerchant; set => mwsMerchant = value; }
        internal ReportType ReportType1 { get => reportType; set => reportType = value; }
        public List<object> AmzOrders { get => amzOrders; set => amzOrders = value; }

        public enum ReportType
        {
            _GET_FLAT_FILE_ORDERS_DATA_,
            _GET_MERCHANT_LISTINGS_DATA_,
            //_GET_ORDERS_DATA_, // Order 100 not allowed at this time
            _GET_XML_ALL_ORDERS_DATA_BY_LAST_UPDATE_,
        };


        public AmazonOrderProcessing(string accessKey, string secretAccessKey, string serviceUrl, string appName, string appVersion, string code, string merchant)
        {
            mwsAccessKey = accessKey;
            mwsSecretAccessKey = secretAccessKey;
            mwsServiceUrl = serviceUrl;
            mwsAppName = appName;
            mwsAppVersion = appVersion;
            mwsCode = code;
            mwsMerchant = merchant;
            

            //Thread.Sleep(15000);
        }

        private MarketplaceWebServiceClient GetClient()
        {
            mwsConfig = new MarketplaceWebServiceConfig();
            mwsConfig.ServiceURL = this.ServiceUrl;
            mwsConfig.SetUserAgentHeader(this.mwsAppName, this.mwsAppVersion, this.mwsCode, new string[] { });

            return new MarketplaceWebServiceClient(AccessKey, SecretAccessKey,AppName,AppVersion,mwsConfig);
        }



        public RequestReportResult GetRequestOrderResult(DateTime startDate, DateTime endDate, ReportType reportType)
        {
            
            RequestReportRequest reportRequestRequest = new RequestReportRequest();
            reportRequestRequest.Merchant = mwsMerchant;
            //reportRequestRequest.
            reportRequestRequest.ReportType = reportType.ToString();

            // If this is an order report, get only last week
            if (reportType == ReportType._GET_FLAT_FILE_ORDERS_DATA_)
            {
                reportRequestRequest.StartDate = startDate;
                reportRequestRequest.EndDate = endDate;
            }


            return GetClient().RequestReport(reportRequestRequest).RequestReportResult;
        }


        public List<ReportRequestInfo> GetReportRequests()
        {
                GetReportRequestListRequest reportlistReqests = new GetReportRequestListRequest();
                reportlistReqests.Merchant = mwsMerchant;
                GetReportRequestListResponse reportlistResponse = GetClient().GetReportRequestList(reportlistReqests);
                return reportlistResponse.GetReportRequestListResult.ReportRequestInfo;
        }

        public FileInfo GetReportFile(ReportRequestInfo reportInfo)
        {
            GetReportRequest reportRequest = new GetReportRequest();
            reportRequest.Merchant = mwsMerchant;
            reportRequest.WithReportId(reportInfo.GeneratedReportId);

            GetReportResponse reportResponse = new GetReportResponse();

            string path = "Z:/Repos/amazonOrders/amazonOrders/ReportDownloaded/Report-" + reportInfo.ReportType + "-" + reportInfo.GeneratedReportId + ".txt";

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                reportRequest.Report = stream;
                reportResponse = GetClient().GetReport(reportRequest);
            }

            return new FileInfo(path);
        }


        
        

    }
}
