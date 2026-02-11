using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EvolveCDB.Model;
using Microsoft.Extensions.Options;

namespace EvolveCDB.Services
{
    public class CardService(IOptionsMonitor<CardListOptions> cardList, ILogger<CardService> logger)
    {
        private string _s3_endpoint = Environment.GetEnvironmentVariable("S3_ENDPOINT") ?? string.Empty;
        private string _s3_access_key = Environment.GetEnvironmentVariable("S3_ACCESS") ?? string.Empty;
        private string _s3_secret_key = Environment.GetEnvironmentVariable("S3_SECRET") ?? string.Empty;
        private AmazonS3Client? _s3;
        private const string _bucketName = "evolvecdb";
        private IOptionsMonitor<CardListOptions> _optionsMonitor = cardList;

        public Card? GetSingleCardById(string cardId) => _optionsMonitor.CurrentValue.Cards.FirstOrDefault(card => card.CardId.Equals(cardId, StringComparison.InvariantCultureIgnoreCase));

        public Card[] GetAllCards(string? cardIdContains, string? nameLike, string? kind, string? classType, int? cost)
        {
            IEnumerable<Card> cardList = _optionsMonitor.CurrentValue.Cards.ToList();

            if (cardIdContains is not null)
            {
                cardList = cardList.Where(card => card.CardId.Contains(cardIdContains, StringComparison.InvariantCultureIgnoreCase));
            }

            if (nameLike is not null)
            {
                cardList = cardList.Where(card => card.Name.Contains(nameLike, StringComparison.InvariantCultureIgnoreCase) ||
                    (card.AlternateDetails is not null && card.AlternateDetails.Name.Contains(nameLike, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (kind is not null)
            {
                cardList = cardList.Where(card => card.Kind.Equals(kind, StringComparison.InvariantCultureIgnoreCase));
            }

            if (classType is not null)
            {
                cardList = cardList.Where(card => card.ClassType.Equals(classType, StringComparison.InvariantCultureIgnoreCase));
            }

            if (cost is not null)
            {
                cardList = cardList.Where(card => card.Cost == cost);
            }

            return [.. cardList];
        }
    
        public Card? SearchForCardName(string nameSearch)
        {
            Card? cardResult = null;

            var cardNameArray = _optionsMonitor.CurrentValue.Cards.Select(card => card.Name).ToArray();
            Fastenshtein.Levenshtein lev = new(nameSearch);

            int lowestDistance = 100;
            foreach (var item in cardNameArray)
            {
                int levenshteinDistance = lev.DistanceFrom(item);
                if (levenshteinDistance < lowestDistance)
                {
                    cardResult = _optionsMonitor.CurrentValue.Cards.First(card => card.Name.Equals(item));
                    lowestDistance = levenshteinDistance;
                }
            }

            return cardResult;
        }

        public Card? SearchForTokenName(string nameToSearch)
        {
            Card? cardResult = null;

            var cardNameArray = _optionsMonitor.CurrentValue.Cards.Where(card => card.Kind.Contains("token", StringComparison.InvariantCultureIgnoreCase))
                .Select(card => card.Name).ToArray();

            if(cardNameArray == null || cardNameArray.Length <= 0)
                return null;
                
            Fastenshtein.Levenshtein lev = new(nameToSearch);

            int lowestDistance = 100;
            foreach (var item in cardNameArray)
            {
                int levenshteinDistance = lev.DistanceFrom(item);
                if (levenshteinDistance < lowestDistance)
                {
                    cardResult = _optionsMonitor.CurrentValue.Cards.First(card => card.Name.Equals(item));
                    lowestDistance = levenshteinDistance;
                }
            }

            return cardResult;
        }
    
        public (Stream, DateTime) GetCardImage(string cardId)
        {
            if(_s3 == null)
            {
                logger?.LogInformation("Initializing S3...");
                if (!string.IsNullOrEmpty(_s3_secret_key))
                {
                    var creds = new BasicAWSCredentials(_s3_access_key, _s3_secret_key);

                    _s3 = new AmazonS3Client(creds, new AmazonS3Config()
                    {
                        ServiceURL = _s3_endpoint,
                    });
                }
                else
                {
                    _s3 = new AmazonS3Client(new AmazonS3Config()
                    {
                        ServiceURL = _s3_endpoint,
                    });
                }
            }

            List<string> listOfKeys = [];
            string contToken = string.Empty;
            string cardIdWOExt = cardId.Replace(".PNG", ".png").Replace(".png", "");
            logger?.LogInformation($"Card ID received: {cardIdWOExt}");

            do
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                };

                if(!string.IsNullOrWhiteSpace(contToken))
                {
                    listRequest.ContinuationToken = contToken;
                }

                var listResponse = _s3!.ListObjectsV2Async(listRequest).ConfigureAwait(false).GetAwaiter().GetResult();

                if (listResponse is not null && listResponse.S3Objects is null)
                {
                    throw new FileNotFoundException($"Image for card {cardIdWOExt} not found.");
                }

                listOfKeys.AddRange([.. listResponse!.S3Objects!.Select(obj => obj.Key.Replace(".png", ""))]);

                contToken = listResponse!.NextContinuationToken;
            }
            while (contToken is not null);

            if (!listOfKeys.Contains(cardIdWOExt))
            {
                throw new FileNotFoundException($"Image for card {cardIdWOExt} not found.");
            }

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{cardIdWOExt}.png"
            };

            var response = _s3.GetObjectAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new FileNotFoundException($"Could not retrieve image for card {cardIdWOExt}.");
            }

            logger?.LogInformation($"Found card for Id {cardIdWOExt}!");

            MemoryStream memStream = new();
            response.ResponseStream.CopyTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            var lastModifiedDateTime = response?.LastModified ?? DateTime.UtcNow;            
            logger?.LogInformation($"Card last modified: {lastModifiedDateTime}");

            return (memStream, lastModifiedDateTime);
        }
    }
}
