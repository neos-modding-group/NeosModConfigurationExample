
using Newtonsoft.Json;

namespace NeosModConfigurationExample
{
    public class CustomClass
    {
        public CustomClass(string a, string b, string c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public string a;
        public string b;
        [JsonIgnore]
        public string c;

        public override string ToString()
        {
            return $"{a}, {b}, {c}";
        }
    }
}
