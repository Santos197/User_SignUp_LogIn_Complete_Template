using Services.DTOs;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces;

public interface IAccountLogic
{
    bool Register(UserAuthenticationRequest request, string Role);
}