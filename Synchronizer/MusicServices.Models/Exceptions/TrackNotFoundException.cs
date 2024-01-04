namespace MusicServices.Models.Exceptions;

public class TrackNotFoundException : Exception
{
    public TrackNotFoundException() : base("Track not found")
    {
    }
}