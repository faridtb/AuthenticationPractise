namespace Country.Api.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
