﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Test_CreateDLL
{

    public class inputData
    {
        public int a { get; set; }
        public int b { get; set; }
        public string c { get; set; }
    }
    public class Account
    {
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; }
        public int RcvData { get; set; }
        public object input { get; set; }
    }

    public class dllTest
    {
        public async Task<object> get(object input)
        {
            Account account = new Account
            {
                Email = "james@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin",
                    "abcd"
                },
                RcvData = (int)input, // (int)input
                input = input
            };
            return account;
        }
        public async Task<object> add(object input)
        {
            string json = JsonConvert.SerializeObject(input, Formatting.Indented);
            inputData ipd = JsonConvert.DeserializeObject<inputData>(json);
            Account account = new Account
            {
                Email = "james@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin",
                    "abcd"
                },
                RcvData = 12345 + ipd.a, // (int)input
                input = input
            };
            return account;
        }
        
    }
}
