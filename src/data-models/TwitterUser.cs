public class TwitterUser
{
    public int id { get; }
    public string id_str { get; }
    public string name { get; }
    public string screen_name { get; }
    public string location { get; }
    public string description { get; }
    public string url { get; }
    public int followers_count { get; }
    public int friends_count { get; }
    public int listed_count { get; }
    public string created_at { get; }
    public int favourites_count { get; }
    public string utc_offset { get; }
    public string time_zone { get; }
    public bool geo_enabled { get; }
    public bool verified { get; }
    public int statuses_count { get; }
    public string lang { get; }
    public bool contributors_enabled { get; }
    public bool is_translator { get; }
    public bool is_translation_enabled { get; }

    public string profile_background_color { get; }
    public string profile_background_image_url { get; }
    public string profile_background_image_url_https { get; }
    public string profile_background_tile { get; }
    public string profile_image_url { get; }
    public string profile_image_url_https { get; }
    public string profile_banner_url { get; }
    public string profile_link_color { get; }
    public string profile_sidebar_border_color { get; }
    public string profile_sidebar_fill_color { get; }
    public string profile_text_color { get; }
    public bool profile_use_background_image { get; }
    public bool has_extended_profile { get; }
    public bool default_profile { get; }
    public bool default_profile_image { get; }
    public string following { get; }
    public bool follow_request_sent { get; }
    public string notifications { get; }
    public string translator_type { get; }
}