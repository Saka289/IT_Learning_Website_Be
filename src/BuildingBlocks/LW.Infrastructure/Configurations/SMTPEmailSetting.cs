﻿using LW.Contracts.Configurations;

namespace LW.Infrastructure.Configurations;

public class SMTPEmailSetting : ISMTPEmailSetting
{
    public string DisplayName { get; set; }
    public bool EnableVerification { get; set; }
    public string From { get; set; }
    public string SMTPServer { get; set; }
    public bool UseSsl { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}