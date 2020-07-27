﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordManagerApp.Models.Entities
{
    [Table("login_data")]
    public class LoginData
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
       
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [Column("login")]
        public string Login { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }
        [Column("website")]
        public string Website { get; set; }
        [Required]
        [Column("compromised")]
        public int Compromised { get; set; }
        [Required]
        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }


        public ICollection<SharedLoginData> SharedLoginDatas { get; set; }





    }
}
