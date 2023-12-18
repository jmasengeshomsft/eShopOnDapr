// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
namespace EshopOnAI.ProductGenerator.Models  
{ 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Merchandise
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        // public int typeId { get; set; }
        public string brand { get; set; }
        // public int brandId { get; set; }
        public string prompt { get; set; }
        public decimal price { get; set; }
        public int availableStock { get; set; }
    }

    public class Root
    {
        public List<Suggestion> suggestions { get; set; }
    }

    public class Suggestion
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Merchandise> merchandises { get; set; }
    }
}

