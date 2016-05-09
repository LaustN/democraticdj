using System.Collections.Generic;
using Newtonsoft.Json;

namespace Democraticdj.Model.Spotify
{
  public class SpotifySearchResponse
  {
    [JsonProperty("tracks")]
    public TrackSearchResult TrackSearchResult { get; set; }
  }


  public class TrackSearchResult
  {
    [JsonProperty("href")]
    public string RequestedUrl { get; set; }

    [JsonProperty("items")]
    public Track[] Tracks { get; set; }
    
  }

  public class Track : ItemBase

  {
    [JsonProperty("album")]
    public Album Album { get; set; }

    [JsonProperty("artists")]
    public Artist[] Artists { get; set; } 

    [JsonProperty("disc_number")]
    public int DiscNumber { get; set; }

    [JsonProperty("duration_ms")]
    public int DurationMs { get; set; }
    
    [JsonProperty("explicit")]
    public bool Explicit { get; set; }
    
    [JsonProperty("external_ids")]
    public Dictionary<string, string> ExternalIds { get; set; }

    [JsonProperty("popularity")]
    public int Popularity { get; set; }

    [JsonProperty("preview_url")]
    public string PreviewUrl { get; set; }

    [JsonProperty("track_number")]
    public int TrackNumber { get; set; }


  }

  public class ItemBase
  {
    [JsonProperty("external_urls")]
    public Dictionary<string,string> ExternalUrls { get; set; } 

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("uri")]
    public string Uri { get; set; }
    
  }

  public class Album : ItemBase
  {
    [JsonProperty("album_type")]
    public string AlbumType { get; set; }

    [JsonProperty("available_markets")]
    public string[] AvailableMarkets { get; set; }

    [JsonProperty("images")]
    public ImageReference[] Images { get; set; }

  }

  public class ImageReference
  {
    [JsonProperty("height")]
    public int Height;
    [JsonProperty("width")]
    public int Width;
    [JsonProperty("url")]
    public string Url { get; set; }
  }

  public class Artist : ItemBase
  {
    
  }
}