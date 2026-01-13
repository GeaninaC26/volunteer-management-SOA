using System.Text.Json.Serialization;

namespace VolunteerManagement.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender { Male, Female, Other }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StudyType { Bachelor, Master, Doctorate }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StudyLanguage { Romanian, English, German, Hungarian }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShirtSize { XS, S, M, L, XL, XXL, Special }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RecruitingStatus { Open, Scheduled, Pending, Rejected, Accepted }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VolunteerStatus { Active, Inactive }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Department { HumanResources, Events, ExternalRelations, ImageAndPR }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Diet { Omnivor, Vegetarian, Vegan }
}
