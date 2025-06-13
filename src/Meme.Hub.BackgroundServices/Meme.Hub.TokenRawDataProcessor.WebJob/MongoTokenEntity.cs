namespace Meme.Hub.TokenRawDataProcessor.WebJob
{
    public class MongoTokenEntity
    {
        public string Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string StrValue { get; set; }
        public DateTime ExpiresOn { get; internal set; }
    }
}
