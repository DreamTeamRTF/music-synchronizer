using Synchronizer.WebApp.Models.Synchronizer.Playlists;

namespace Synchronizer.WebApp.Models.Synchronizer.LinkedAccounts;

public class AccountToLinkModel
{
    public string Login { get; set; }
    public string Password { get; set; }
    public MusicServiceTypeModel MusicServiceTypeModel { get; set; }
}