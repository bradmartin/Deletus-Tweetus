public class TwitterEndpoints
{
    public static string API_BASE_URL
    {
        get { return "https://api.twitter.com/"; }
    }

    public static string REST_ROOT_URL
    {
        get { return "https://api.twitter.com/1.1/"; }
    }

    public static string PUB_STREAM
    {
        get { return "https://stream.twitter.com/1.1/"; }
    }

    public static string USER_STREAM
    {
        get { return "https://userstream.twitter.com/1.1/"; }
    }

    public static string SITE_STREAM
    {
        get { return "https://sitestream.twitter.com/1.1/"; }
    }

    public static string MEDIA_UPLOAD
    {
        get { return "https://upload.twitter.com/1.1/"; }
    }

    public static string OA_REQ
    {
        get { return "https://api.twitter.com/oauth/request_token"; }
    }

    public static string OA_ACCESS
    {
        get { return "https://api.twitter.com/oauth/access_token"; }
    }
}