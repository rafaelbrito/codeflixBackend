
namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime
{
    internal static class DateTimeExtensions
    {
        public static System.DateTime TrimMillisseconds(
            this System.DateTime datetime)
        {
            return new System.DateTime(
                  datetime.Year,
                  datetime.Month,
                  datetime.Day,
                  datetime.Hour,
                  datetime.Minute,
                  datetime.Second,
                  0,
                  datetime.Kind
                  );
        }

    }
}
