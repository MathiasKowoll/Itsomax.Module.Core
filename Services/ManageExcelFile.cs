using System;
using System.Collections.Generic;
using System.IO;
using Itsomax.Module.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Itsomax.Module.Core.Services
{
    public class ManageExcelFile : IManageExcelFile
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ManageExcelFile(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

		public IList<string> GenerateExcelName(string reportName,DateTime reportDate,string warehouse)
        {
            //const string softlandPath = "ExcelTemplate";
			var sourceFilename = reportName == "SalidaSoftland"+warehouse ? @"SalidaSoftland"+warehouse+".xlsx" : "";
            //var sourceFile = Path.Combine(_hostingEnvironment.WebRootPath,softlandPath, sourceFilename);
            var destFilename = Path.Combine(_hostingEnvironment.WebRootPath, "Temp", reportName + reportDate.ToString("yyyyMMdd")+".xlsx");
            if (sourceFilename == "")
            {
				var nameList = new List<string> { null, null };
				return nameList;
            }
            else
            {
				//File.Copy(sourceFile,destFilename,true);
                var nameList = new List<string> {destFilename, reportName + reportDate.ToString("yyyyMMdd") + ".xlsx"};
                return nameList;
            }
            
        }
        /*
        public string CreateExcelFile(string exelName)
        {
            var tempDir = "Temp";
            var path = Path.Combine(_hostingEnvironment.WebRootPath,tempDir);
            var newFile = Path.Combine(path, exelName);
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(newFile,FileMode.Create,FileAccess.Write))
            {
                var fileExtention = Path.GetExtension(newFile).ToLower();
                ISheet sheet;
                if (fileExtention == ".xls")
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(fs);
                    sheet = workbook.GetSheetAt(0);

                }
            }

            return newFile;
        }
        */

        /*
        public string WriteExcelFile(string fileName, IList<dynamic> list)
        {
            var properties =list.GetType().GetProperties();

            return fileName;
        }
        */
    }
}

