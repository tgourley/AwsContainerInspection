using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace AwsContainerInspection
{
    public static class AwsContainerService
    {
        public static AwsContainerMetadata GetMetadata()
        {
            try
            {
                string metadataFileLocation = Environment.GetEnvironmentVariable("ECS_CONTAINER_METADATA_FILE");

                if (!string.IsNullOrWhiteSpace(metadataFileLocation))
                {
                    FileInfo metadataFileInfo = new FileInfo(metadataFileLocation);

                    if (metadataFileInfo.Exists)
                    {
                        StreamReader metadataFileStream = metadataFileInfo.OpenText();

                        string metadataFileContents = metadataFileStream.ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(metadataFileContents))
                        {
                            if (IsValidJson(metadataFileContents))
                            {
                                try
                                {
                                    var metadataObject = JsonConvert.DeserializeObject<AwsContainerMetadata>(metadataFileContents);
                                    return metadataObject;
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                }
            }
            catch(Exception)
            { }

            return null;
        }

        //https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        //https://stackoverflow.com/a/14977915

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    //Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    //Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
