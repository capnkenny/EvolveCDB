using EvolveCDB.Model;
using EvolveCDB.Services;

namespace EvolveCDB.Endpoints
{
    public class DeckEndpoints(DeckService _service)
    {
        public async Task<AbbreviatedDeckList?> GetShortenedDeckListFromCode(string code) => await _service.GetShortenedDeckListFromCode(code);


        public async Task<DeckList?> GetDeckFromCode(string code) => await _service.GetDeckFromCode(code);
    }


}
