using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

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
        public async Task<IResult> GetImageByCardId(string cardId, CancellationToken token = default)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _bucketName
            };

            var listResponse = await _s3.ListObjectsV2Async(listRequest, token);

            if (listResponse is not null && listResponse.S3Objects is null)
            {
                return Results.NotFound($"Image for card {cardId} not found.");
            }

            List<string> listOfKeys = [.. listResponse!.S3Objects!.Select(obj => obj.Key.Replace(".png", ""))];
            if (!listOfKeys.Contains(cardId.ToUpperInvariant()))
            {
                return Results.NotFound($"Image for card {cardId} not found.");
            }

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{cardId.ToUpperInvariant()}.png"
            };

            var response = await _s3.GetObjectAsync(request, token);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return Results.NotFound($"Could not retrieve image for card {cardId}.");
            }

            byte[] content = [];

            using (var memStream = new MemoryStream())
            {
                response.ResponseStream.CopyTo(memStream);
                response.ResponseStream.Flush();
                content = memStream.ToArray();
            }

            return Results.File(content, "image/png", request.Key);
        }
    }

    //public class CardEndpoints
    //{
    //    private readonly CardService _cardService;

    //    public CardEndpoints(CardService service)
    //    {
    //        _cardService = service;
    //    }

    //    public Card? GetSingleCardById(string cardId) => _cardService.GetSingleCardById(cardId);

    //    public Card[] GetAllCards(string? cardIdContains, string? nameLike, string? kind, string? classType, int? cost) =>
    //        _cardService.GetAllCards(cardIdContains, nameLike, kind, classType, cost);
    //}
}
