﻿using FluentValidation;
using Services.DTOs;

namespace Services.Validators;

public class UserAuthenticationRequestValidator : AbstractValidator<UserAuthenticationRequest>
{
    public UserAuthenticationRequestValidator()
    {
        //RuleFor(u => u.UserName).NotEmpty().NotNull();
        RuleFor(u => u.Password).NotEmpty().NotNull();//.MinimumLength(8);
        }
}