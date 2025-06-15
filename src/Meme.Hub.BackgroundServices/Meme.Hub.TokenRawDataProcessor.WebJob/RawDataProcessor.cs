using Newtonsoft.Json;
using Meme.Domain.Models.TokenModels;

namespace Meme.Hub.TokenRawDataProcessor.WebJob
{

    public class RawDataProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly CosmosDBCacheService _cacheService;

        public RawDataProcessor()
        {
            _httpClient = new HttpClient();
            _cacheService = new CosmosDBCacheService();
        }

        public async Task ProcessTokenAsync(string rawTokenData)
        {
            var rawDataModel = JsonConvert.DeserializeObject<RawTokenDataModel>(rawTokenData);

            if (rawDataModel?.TxType != TokenTransactionType.Create) return;

            if (string.IsNullOrEmpty(rawDataModel?.Uri))
            {
                // log error message
                return;
            }

            var response = await _httpClient.GetAsync(rawDataModel.Uri);
            response.EnsureSuccessStatusCode();

            var tokenData = JsonConvert.DeserializeObject<TokenDataModel>(await response.Content.ReadAsStringAsync()); // await _httpClient.GetData<TokenDataModel>(rawDataModel.Uri);
            if (tokenData == null)
            {
                // log error message
                return;
            }

            tokenData.RawData = rawDataModel;
            await _cacheService.AddItemToList(tokenData, TimeSpan.FromMinutes(30));
        }
    }
}
