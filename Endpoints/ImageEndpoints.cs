using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
namespace EvolveCDB.Endpoints
{
    public class ImageEndpoints
    {
        private readonly IAmazonS3 _s3;
        private const string _bucketName = "evolvecdb";

        public ImageEndpoints()
        {
            string s3_endpoint = Environment.GetEnvironmentVariable("S3_ENDPOINT") ?? string.Empty;
            string s3_access_key = Environment.GetEnvironmentVariable("S3_ACCESS") ?? string.Empty;
            string s3_secret_key = Environment.GetEnvironmentVariable("S3_SECRET") ?? string.Empty;

            if (!string.IsNullOrEmpty(s3_secret_key))
            {
                var creds = new BasicAWSCredentials(s3_access_key, s3_secret_key);

                _s3 = new AmazonS3Client(creds, new AmazonS3Config()
                {
                    ServiceURL = s3_endpoint,
                });
            }
        }

        //im lazy rn so the code will go here instead of in a service
        //TODO: move to service.
        public Stream GetImageByCardId(string cardId, HttpContext context)
        {
            List<string> listOfKeys = [];
            string contToken = null;
            string cardIdWOExt = cardId.ToLowerInvariant().Replace(".png","");
            string cardCaps = cardIdWOExt.ToUpperInvariant();
            do
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    ContinuationToken = contToken,
                };

                var listResponse = _s3.ListObjectsV2Async(listRequest).ConfigureAwait(false).GetAwaiter().GetResult();

                if (listResponse is not null && listResponse.S3Objects is null)
                {
                    throw new FileNotFoundException($"Image for card {cardCaps} not found.");
                }

                listOfKeys.AddRange([.. listResponse!.S3Objects!.Select(obj => obj.Key.Replace(".png", ""))]);

                contToken = listResponse!.NextContinuationToken;
            }
            while (contToken is not null);

            if (!listOfKeys.Contains(cardCaps))
            {
                throw new FileNotFoundException($"Image for card {cardCaps} not found.");
            }

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{cardCaps}.png"
            };

            var response = _s3.GetObjectAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new FileNotFoundException($"Could not retrieve image for card {cardCaps}.");
            }

            MemoryStream memStream = new();
            response.ResponseStream.CopyTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }
    }
}
