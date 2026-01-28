namespace Ecommerce.Core;

    public record ProductSpecParams : BaseSpecParams
    {
        public int? BrandId { get; init; }
        public int? CategoryId { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }

        public override int PageSize
        {
            get => base.PageSize == 10 ? 20 : base.PageSize; // Maintain the old default of 20
            init => base.PageSize = value;
        }
    }
