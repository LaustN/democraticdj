using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Democraticdj.Model.Spotify
{
  public class SpotifyPlaylistsResponse
  {
    [JsonProperty("href")]
    public string RequestedUrl { get; set; }

    [JsonProperty("items")]
    public Playlist[] PlayLists { get; set; }
  }

  public class Playlist : ItemBase
  {
    [JsonProperty("collaborative")]
    public bool IsCollaborative { get; set; }

    [JsonProperty("public")]
    public bool IsPublic { get; set; }

    [JsonProperty("images")]
    public ImageReference[] Images { get; set; }

    [JsonProperty("owner")]
    public ItemBase Owner { get; set; }

    [JsonProperty("snapshot_id")]
    public string SnapshotId { get; set; }

    [JsonProperty("tracks")]
    public SizedReference SizedReference { get; set; }

  }

  public class SizedReference
  {
    [JsonProperty("href")]
    public string TracksListReference { get; set; }

    [JsonProperty("total")]
    public int TrackCount { get; set; }
  }

  public class CreatePlaylistRequest
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("public")]
    public bool Public { get; set; }
  }

  public class PlaylistTracksResponse
  {
    [JsonProperty("href")]
    public string FullResultHref { get; set; }

    [JsonProperty("items")]
    public PlaylistTrackObject[] Tracks { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("next")]
    public string NextResultHref { get; set; }

    [JsonProperty("offset")]
    public int Offset { get; set; }

    [JsonProperty("previous")]
    public string PreviousResultHref { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

  }

  public class PlaylistTrackObject
  {
    [JsonProperty("added_at")]
    public string AddedAt { get; set; }

    [JsonProperty("added_by")]
    public SpotifyUser AddedBy { get; set; }

    [JsonProperty("is_local")]
    public bool IsLocal { get; set; }

    [JsonProperty("track")]
    public Track Track { get; set; }

  }
}