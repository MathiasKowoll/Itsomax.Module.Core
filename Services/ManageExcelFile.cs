using System.Collections.Generic;
using System.IO;
using Itsomax.Module.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Itsomax.Module.Core.Services
{
    public class ManageExcelFile : IManageExcelFile
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ManageExcelFile(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

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


        public string WriteExcelFile(string fileName, IList<dynamic> list)
        {
            var properties =list.GetType().GetProperties();

            return fileName;
        }
    }
}

