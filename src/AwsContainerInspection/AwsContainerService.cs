using AwsContainerInspection.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace AwsContainerInspection
{
    public class AwsContainerService
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public static AwsContainerFile GetMetadataFromFile()
        {
            try
            {
                string metadataFileLocation = Environment.GetEnvironmentVariable("ECS_CONTAINER_METADATA_FILE");

                if (string.IsNullOrWhiteSpace(metadataFileLocation))
                {
                    return null;
                }

                FileInfo metadataFileInfo = new FileInfo(metadataFileLocation);

                if (!metadataFileInfo.Exists)
                {
                    return null;
                }

                string metadataFileContents;
                using (StreamReader metadataFileStream = metadataFileInfo.OpenText())
                {
                    metadataFileContents = metadataFileStream.ReadToEnd();
                }

                if (IsValidJson(metadataFileContents))
                {
                    return JsonConvert.DeserializeObject<AwsContainerFile>(metadataFileContents);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }

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
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }

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
            catch (Exception Ex)
            {
                Logger.Error(Ex.Message);
                Logger.Error(Ex.StackTrace);
            }

            return null;
        }

        //https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        //https://stackoverflow.com/a/14977915

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }

            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Logger.Error(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
