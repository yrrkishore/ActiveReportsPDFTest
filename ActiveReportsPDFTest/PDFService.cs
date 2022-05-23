using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Mime;
using System.Text.Encodings.Web;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace ActiveReportsPDFTest
{
    public static class ActiveReportsPDFService
    {
        [FunctionName("DownloadPdf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {
            try
            {
                // Provide the page report you want to render.
                //System.IO.FileInfo rptPath = new System.IO.FileInfo(@"Report\Test_GTIN_Checkedin.rdlx");
                System.IO.FileInfo rptPath = new System.IO.FileInfo(Path.Combine(context.FunctionAppDirectory, "Report", "Test_PDF.rdlx"));
                log.LogInformation("rptPath ==> " + rptPath.FullName);
                GrapeCity.ActiveReports.PageReport pageReport = new GrapeCity.ActiveReports.PageReport(rptPath);
                log.LogInformation("pageReport path is set..");               
                // Create an output directory.
                System.IO.DirectoryInfo outputDirectory = new System.IO.DirectoryInfo(Path.Combine(context.FunctionAppDirectory, "Report", "report-output"));
                log.LogInformation("outputDirectory ==> " + outputDirectory);
                outputDirectory.Create();
                log.LogInformation("outputDirectory created");
                // Provide settings for your rendering output.
                GrapeCity.ActiveReports.Export.Pdf.Page.Settings pdfSetting = new GrapeCity.ActiveReports.Export.Pdf.Page.Settings();
                // Set the rendering extension and render the report.
                GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension pdfRenderingExtension = new GrapeCity.ActiveReports.Export.Pdf.Page.PdfRenderingExtension();
                GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider outputProvider = new GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(outputDirectory.Name));

                // Overwrite output file if it already exists
                outputProvider.OverwriteOutputFile = true;

                pageReport.Document.Render(pdfRenderingExtension, outputProvider, pdfSetting, true);
                log.LogInformation("Document rendered ");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                log.LogError(ex.Message);
                
            }
            return null;            
        }        
    }
    
}
