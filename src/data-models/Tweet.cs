public class Tweet
{
    public string created_at { get; }
    public string id { get; }
    public string id_str { get; }
    public string text { get; }
    public bool truncated { get; }
    public string source { get; }
    public TwitterUser user { get; }
    public string geo { get; }
    public string coordinates { get; }
    public string place { get; }
    public string contributors { get; }
    public string is_quote_status { get; }
    public int retweet_count { get; }
    public int favorite_count { get; }
    public bool favorited { get; }
    public bool retweeted { get; }
    public string lang { get; }
}