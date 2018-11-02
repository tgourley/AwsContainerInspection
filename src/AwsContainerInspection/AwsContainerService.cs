using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace AwsContainerInspection
{
    public static class AwsContainerService
    {
        public static AwsContainerFile GetMetadataFromFile()
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
                                    var metadataObject = JsonConvert.DeserializeObject<AwsContainerFile>(metadataFileContents);
                                    return metadataObject;
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }

            return null;
        }

        public static AwsContainerEndpoint GetMetadataFromEndpoint()
        {
            try
            {
                string endpointUrl = "http://169.254.170.2/v2/metadata";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpointUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var metadata = reader.ReadToEnd();

                    return GetMetadataFromEndpoint(metadata);
                }
            }
            catch (Exception)
            { }

            return null;
        }

        public static AwsContainerEndpoint GetMetadataFromEndpoint(string metadata)
        {
            try
            {
                if (IsValidJson(metadata))
                {
                    var metadataObject = JsonConvert.DeserializeObject<AwsContainerEndpoint>(metadata);
                    return metadataObject;
                }
            }
            catch (Exception)
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
