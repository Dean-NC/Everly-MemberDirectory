using System;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Interfaces
{
    public interface IUrlShortener
    {
        Task<string> Shorten(string url);
    }
}
