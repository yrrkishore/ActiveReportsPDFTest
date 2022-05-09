using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ActiveReportsPDFTest
{
    public static class ActiveReportsPDFService
    {
        [FunctionName("DownloadPdf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                
                // Provide the page report you want to render.
                System.IO.FileInfo rptPath = new System.IO.FileInfo(@"Report\Test_PDF.rdlx");

                GrapeCity.ActiveReports.PageReport pageReport = new GrapeCity.ActiveReports.PageReport(rptPath);

                //pageReport.Document.LocateDataSource += OnLocateDataSource;
                // Create an output directory.
                System.IO.DirectoryInfo outputDirectory = new System.IO.DirectoryInfo(@"C:\MyPDF");
                outputDirectory.Create();

                // Provide settings for your rendering output.
                GrapeCity.ActiveReports.Export.Pdf.Page.Settings pdfSetting = new GrapeCity.ActiveReports.Export.Pdf.Page.Settings();

                // Set the rendering extension and render the report.
                GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension pdfRenderingExtension = new GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension();
                GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider outputProvider = new GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(outputDirectory.Name));

                // Overwrite output file if it already exists
                outputProvider.OverwriteOutputFile = true;

                // Not sure if it is the correct method to call to return pdf result. but it gives the same error 
                // ****"This application will be terminated because it was built without a license for PageReport"
                
                                pageReport.Document.Render(pdfRenderingExtension, outputProvider, pdfSetting);
              
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return null;
            
        }
    }
}
