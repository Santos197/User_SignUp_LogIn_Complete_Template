﻿using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Models.Helpers;

namespace Services.Helpers.MailService;

public class SmtpMailService(IOptions<AppSettings> json) : IMailService
{
    public void SendEmail(string toEmail, string message, string fromTitle = "", string Subject = "")
    {
            var options = json.Value.Smtp;
            toEmail = toEmail.ToLower();
            var email = new MimeMessage();
            var from = new MailboxAddress(fromTitle, options.Email);
            email.From.Add(from);

            var to = MailboxAddress.Parse(toEmail);
            email.To.Add(to);

            email.Subject = Subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = message
            };
            email.Body = bodyBuilder.ToMessageBody();
            var client = new SmtpClient();
            try
            {
                client.Connect(options.Host, options.Port, SecureSocketOptions.StartTls);
                client.Authenticate(options.Email, options.Password);
                client.Send(email);
                client.Disconnect(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
}