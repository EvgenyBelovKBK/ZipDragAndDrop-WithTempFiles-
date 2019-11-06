using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace DragAndDropZip
{
    public class ZipHelper
    {
        public static Dictionary<string, string> OpenArchive(string zipFilePath, Dictionary<string, string> dataToFill)
        {
            using (FileStream fs = File.OpenRead(zipFilePath))
            {
                return ReadArchive(fs,dataToFill);
            }
        }

        private static Dictionary<string,string> ReadArchive(Stream fs,Dictionary<string,string> dataToFill)
        {
            ZipFile zipArchive = new ZipFile(fs);
            foreach (ZipEntry elementInsideZip in zipArchive)
            {
                String ZipArchiveName = elementInsideZip.Name;
                if (ZipArchiveName.Contains(".txt") || !ZipArchiveName.Contains("/"))
                {
                    Stream zipStream = zipArchive.GetInputStream(elementInsideZip);
                    var bytes = new byte[zipStream.Length];
                    zipStream.Read(bytes,0,(int)zipStream.Length);
                    dataToFill.Add(elementInsideZip.Name,Encoding.UTF8.GetString(bytes));
                }
                else if (ZipArchiveName.Contains(".zip"))
                {
                    Stream zipStream = zipArchive.GetInputStream(elementInsideZip);
                    string zipFileExtractPath = Path.GetTempFileName();
                    FileStream extractedZipFile = File.OpenWrite(zipFileExtractPath);
                    zipStream.CopyTo(extractedZipFile);
                    extractedZipFile.Flush();
                    extractedZipFile.Close();
                    try
                    {
                        OpenArchive(zipFileExtractPath,dataToFill);
                    }
                    finally
                    {
                        File.Delete(zipFileExtractPath);
                    }
                }
                else if(ZipArchiveName.Contains("/"))
                {
                    Stream zipStream = zipArchive.GetInputStream(elementInsideZip);
                    string zipDirExtractPath = Path.GetTempPath();
                    string zipFileExtractPath = Path.GetTempFileName();
                    FileStream extractedZipFile = File.OpenWrite(Path.Combine(zipDirExtractPath,zipFileExtractPath));
                    zipStream.CopyTo(extractedZipFile);
                    extractedZipFile.Flush();
                    extractedZipFile.Close();
                    try
                    {
                        OpenArchive(zipFileExtractPath, dataToFill);
                    }
                    finally
                    {
                        File.Delete(zipFileExtractPath);
                    }
                }
            }
            return dataToFill;
        }
    }
}
