using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService
{
    class AuthMessageSender : EmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        

        public AuthMessageSender(EmailConfiguration emailConfig) : base(emailConfig)
        {
            _emailConfig = emailConfig;


        }
       



       
    }

}
