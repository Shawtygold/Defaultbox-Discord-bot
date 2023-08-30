using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Config
{
    internal class JSONReader
    {
        public string Token { get; set; }

        public async Task ReadJson()
        {
            using StreamReader streamReader = new("config.json");
            string json = await streamReader.ReadToEndAsync();
            JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

            if (data == null)
                return;

            Token = data.Token;
        }

        private class JSONStructure
        {
            public string Token { get; set; }
        }
    }
}
