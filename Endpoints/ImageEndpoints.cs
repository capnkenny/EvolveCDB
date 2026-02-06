using EvolveCDB.Services;
namespace EvolveCDB.Endpoints
{
    public class ImageEndpoints
    {
        private readonly CardService _cardService;

        public ImageEndpoints(CardService service)
        {
            _cardService = service;
        }

        public (Stream, DateTime) GetImageByCardId(string cardId) => _cardService.GetCardImage(cardId);
    }
}
