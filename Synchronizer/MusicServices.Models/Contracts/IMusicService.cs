namespace MusicServices.Models.Contracts;

/// <summary>
///     Интерфиейс для всех клиентов, на данный момент Vk.Music и Яндекс.Музыка
/// </summary>
public interface IMusicService
{
    /// <summary>
    ///     Метод для получения всех треков из плейлиста
    /// </summary>
    public Task<Track[]> GetPlaylistTracksAsync(PlaylistRequest request);

    /// <summary>
    ///     Метод для получения всех плейлистов созданых пользователем
    /// </summary>
    public Task<Playlist[]> GetOwnPlaylistsAsync(OwnPlaylistsRequest request);
    
    /// <summary>
    ///     Метод для сохранения плейлиста из другого сервиса
    /// </summary>
    public Task<Playlist> AddPlaylistAsync(PlaylistToAddRequest request);
    
    public Task<Playlist?> FindPlaylistByIdAsync(FindPlaylistByIdRequest request);
}