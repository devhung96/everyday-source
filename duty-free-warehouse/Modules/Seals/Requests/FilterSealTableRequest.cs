namespace Project.Modules.Seals.Requests
{
    public class FilterSealTableRequest
    {
        public string ExportDateTimeForm { get; set; }
        public string ExportDateTimeTo { get; set; }
        public string FlightDateTimeForm { get; set; }
        public string FlightDateTimeTo { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Search { get; set; }
        public int? StatusSeal { get; set; }
    }
}
